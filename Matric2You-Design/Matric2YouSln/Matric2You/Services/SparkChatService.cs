using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using OpenAI.Chat;
using System.ClientModel;
using System.Text.RegularExpressions;

namespace Matric2You.Services
{
    public class SparkChatService
    {
        private readonly ChatClient _chatClient;
        private readonly SearchClient _searchClient;

        public SparkChatService(
            string endpoint,
            string deploymentName,
            string searchEndpoint,
            string searchIndex,
            string? searchApiKey = null,
            string? openAiApiKey = null)
        {
            // Initialize Azure OpenAI client
            AzureOpenAIClient azureClient = !string.IsNullOrEmpty(openAiApiKey)
                ? new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(openAiApiKey))
                : new AzureOpenAIClient(new Uri(endpoint), new DefaultAzureCredential());

            _chatClient = azureClient.GetChatClient(deploymentName);

            // Initialize Azure AI Search client
            _searchClient = string.IsNullOrEmpty(searchApiKey)
                ? new SearchClient(new Uri(searchEndpoint), searchIndex, new DefaultAzureCredential())
                : new SearchClient(new Uri(searchEndpoint), searchIndex, new AzureKeyCredential(searchApiKey));
        }

        public async Task<string> GetChatCompletionWithDataAsync(string userMessage)
        {
            // Step 1: Enhanced search using user input directly
            SearchOptions searchOptions = new()
            {
                Size = 10,
                IncludeTotalCount = true
            };

            SearchResults<SearchDocument> searchResults = await _searchClient.SearchAsync<SearchDocument>(userMessage, searchOptions);

            if (searchResults.TotalCount == 0)
            {
                // Fallback search strategies
                var keywords = await ExtractKeywordsAsync(userMessage);
                searchResults = await _searchClient.SearchAsync<SearchDocument>(keywords, searchOptions);
            }

            // Step 2: Process search results
            List<string> searchContext = new();

            await foreach (SearchResult<SearchDocument> result in searchResults.GetResultsAsync())
            {
                var (content, title) = ExtractContentAndTitle(result.Document);

                if (!string.IsNullOrEmpty(content))
                {
                    if (content.Length > 1000)
                        content = content.Substring(0, 1000) + "...";
                    searchContext.Add($"Source: {title}\nContent: {content}");
                }
            }

            // Step 3: Generate response with context
            string guidance = "Respond in plain text only. Do not use Markdown or LaTeX. Do not escape parentheses or brackets; write ( ), [ ] directly. Avoid putting backslashes before symbols.";

            string contextText = searchContext.Any()
                ? guidance + " Use the key words from the user input and respond based on the user question. Here is relevant information from the knowledge base:\n\n" +
                  string.Join("\n\n---\n\n", searchContext) + "\n\n"
                : guidance + " Use the key words from the user input and respond based on the user question. No relevant information found in knowledge base.";

            List<ChatMessage> messages = new()
            {
                new SystemChatMessage(contextText),
                new UserChatMessage(userMessage)
            };

            ChatCompletionOptions options = new()
            {
                Temperature = 0.7f,
                TopP = 0.95f,
                FrequencyPenalty = 0,
                PresencePenalty = 0
            };

            ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, options);

            var raw = completion.Content[0].Text;
            return SanitizeModelOutput(raw);
        }

        // add overload that accepts history and (optionally) allows generative supplement
        public async Task<string> GetChatCompletionWithDataAsync(
            string userMessage,
            IList<ChatMessage>? history,
            bool allowGenerativeSupplement = true)
        {
            // Step 1: Retrieve
            SearchOptions searchOptions = new()
            {
                Size = 10,
                IncludeTotalCount = true
            };
            SearchResults<SearchDocument> searchResults =
                await _searchClient.SearchAsync<SearchDocument>(userMessage, searchOptions);

            if (searchResults.TotalCount == 0 && history is { Count: > 0 })
            {
                // fallback: try with last assistant/user content to improve recall
                string backoffQuery = userMessage + " " +
                                      string.Join(" ",
                                          history.Reverse().OfType<AssistantChatMessage>()
                                              .Take(1).Select(m => m.Content[0].Text));
                searchResults = await _searchClient.SearchAsync<SearchDocument>(backoffQuery, searchOptions);
            }

            // Step 2: Build grounded context
            List<string> searchContext = new();
            await foreach (SearchResult<SearchDocument> result in searchResults.GetResultsAsync())
            {
                var (content, title) = ExtractContentAndTitle(result.Document);
                if (!string.IsNullOrWhiteSpace(content))
                {
                    if (content.Length > 1000) content = content.Substring(0, 1000) + "...";
                    searchContext.Add($"Source: {title}\nContent: {content}");
                }
            }

            string groundingRule = allowGenerativeSupplement
                ? "Prefer provided sources. You may add general knowledge when needed, but keep it consistent with the sources."
                : "Answer only from the provided sources; if insufficient, say so.";

            // MODIFIED: Encourage the model to use LaTeX for math.
            string guidance = "For mathematical expressions, equations, and formulas, use LaTeX syntax. For all other text, respond in plain text.";

            string contextText = searchContext.Any()
                ? $"{guidance} {groundingRule}\n\nSources:\n\n{string.Join("\n\n---\n\n", searchContext)}\n\n"
                : $"{guidance} {groundingRule}\n\nNo relevant sources found.";

            // Step 3: Compose chat with history + current user turn
            var messages = new List<ChatMessage> { new SystemChatMessage(contextText) };
            if (history is { Count: > 0 })
                messages.AddRange(history);
            messages.Add(new UserChatMessage(userMessage));

            ChatCompletionOptions options = new()
            {
                Temperature = 0.7f,
                TopP = 0.95f,
                FrequencyPenalty = 0,
                PresencePenalty = 0
            };

            ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, options);
            var raw = completion.Content[0].Text;
            return SanitizeModelOutput(raw);
        }

        private async Task<string> ExtractKeywordsAsync(string userInput)
        {
            List<ChatMessage> keywordMessages = new()
            {
                new SystemChatMessage("use the key words from the user input scan through the relavant information and respond based on the user question and keep it in plain text. Do not use LaTeX or Markdown and do not escape parentheses or brackets."),
                new UserChatMessage(userInput)
            };

            ChatCompletion keywordCompletion = await _chatClient.CompleteChatAsync(keywordMessages, new ChatCompletionOptions { Temperature = 0.1f });
            return keywordCompletion.Content[0].Text.Trim();
        }

        private static (string content, string title) ExtractContentAndTitle(SearchDocument document)
        {
            string content = "";
            string title = "";

            string[] contentFields = { "content", "Content", "text", "Text", "body", "Body", "description", "Description" };
            string[] titleFields = { "title", "Title", "name", "Name", "filename", "fileName", "FileName" };

            foreach (var fieldName in contentFields)
            {
                if (document.TryGetValue(fieldName, out object? contentObj) && contentObj != null)
                {
                    content = contentObj.ToString() ?? "";
                    break;
                }
            }

            foreach (var fieldName in titleFields)
            {
                if (document.TryGetValue(fieldName, out object? titleObj) && titleObj != null)
                {
                    title = titleObj.ToString() ?? "";
                    break;
                }
            }

            if (string.IsNullOrEmpty(content))
            {
                var firstContentField = document.FirstOrDefault(f =>
                    !f.Key.Equals("id", StringComparison.OrdinalIgnoreCase) &&
                    !f.Key.Equals("key", StringComparison.OrdinalIgnoreCase) &&
                    f.Value != null &&
                    !string.IsNullOrEmpty(f.Value.ToString()));

                if (firstContentField.Value != null)
                {
                    content = firstContentField.Value.ToString() ?? "";
                }
            }

            return (content, title);
        }

        private static string SanitizeModelOutput(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            // Remove Markdown bold, but leave other symbols for LaTeX detection.
            text = text.Replace("**", "");

            // Add space after punctuation if missing, to help with word wrapping
            text = Regex.Replace(text, @"([.:])([a-zA-Z])", "$1 $2");

            // The aggressive replacements that removed LaTeX markers have been removed.
            // We let the ViewModel decide how to handle the content.

            return text.Trim();
        }
    }
}
