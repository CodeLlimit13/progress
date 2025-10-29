using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Matric2You.Services;
using OpenAI.Chat;
using Microsoft.Maui.Controls;
using System.Text.Json;
using System.Text;
using Matric2You.Models;

namespace Matric2You.ViewModel
{
    public partial class ChatViewModel : ObservableObject
    {
        private readonly SparkChatService _chatService;
        private readonly List<ChatMessage> _history = new();

        [ObservableProperty]
        private string userInput = string.Empty;

        // Replaces the separate Messages + single WebView with a single chat stream
        public ObservableCollection<ChatItem> ChatItems { get; } = new();

        public ChatViewModel(SparkChatService chatService)
        {
            _chatService = chatService;
        }

        [RelayCommand]
        public async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(UserInput)) return;

            var input = UserInput;

            // Append user item
            ChatItems.Add(new ChatItem
            {
                IsUser = true,
                Role = "You",
                Content = input
            });

            _history.Add(new UserChatMessage(input));

            var reply = await _chatService.GetChatCompletionWithDataAsync(input, _history, allowGenerativeSupplement: true);

            _history.Add(new AssistantChatMessage(reply));

            // Append bot item with KaTeX-rendered HTML
            ChatItems.Add(new ChatItem
            {
                IsUser = false,
                Role = "Bot",
                Content = reply,
                // Only build HTML source if the reply contains LaTeX. Otherwise, it will be null.
                HtmlSource = BuildKaTeXHtmlSource(reply)
            });

            UserInput = string.Empty;
        }

        private HtmlWebViewSource? BuildKaTeXHtmlSource(string reply)
        {
            // Only proceed if the reply looks like it contains LaTeX.
            if (!LooksLikeLatex(reply))
            {
                return null; // Return null for plain text.
            }

            string latex = ConvertReplyToLatex(reply);

            string iframeHtml = $@"<!DOCTYPE html>
<html>
<head>
  <meta charset=""utf-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
  <link rel=""stylesheet"" href=""https://cdn.jsdelivr.net/npm/katex@0.16.10/dist/katex.min.css"">
  <script src=""https://cdn.jsdelivr.net/npm/katex@0.16.10/dist/katex.min.js""></script>
  <!-- 1. Add the auto-render extension -->
  <script src=""https://cdn.jsdelivr.net/npm/katex@0.16.10/dist/contrib/auto-render.min.js""></script>
  <style>
    body {{ font-size: 12px; padding: 20px; }}
    .katex-container {{
        white-space: pre-wrap;       /* CSS 2.1 */
        white-space: -moz-pre-wrap;  /* Mozilla, since 1999 */
        white-space: -pre-wrap;      /* Opera 4-6 */
        white-space: -o-pre-wrap;    /* Opera 7 */
        word-wrap: break-word;       /* Internet Explorer 5.5+ */
    }}
  </style>
</head>
<body>
  <!-- 2. The container now gets the raw text, not just a placeholder -->
  <div class=""katex-container"" id=""math""></div>
  <script>
    const rawText = {JsonSerializer.Serialize(latex)};
    const mathEl = document.getElementById('math');
    
    // 3. Set the text and then call the auto-renderer
    mathEl.textContent = rawText;
    window.addEventListener('load', function () {{
      try {{
        renderMathInElement(mathEl, {{
          delimiters: [
            {{left: '$$', right: '$$', display: true}},
            {{left: '\\[', right: '\\]', display: true}},
            {{left: '$', right: '$', display: false}},
            {{left: '\\(', right: '\\)', display: false}}
          ],
          throwOnError: false
        }});
      }} catch (e) {{
        mathEl.textContent = 'KaTeX error: ' + (e && e.message ? e.message : e) + '\n' + rawText;
        console.error(e);
      }}
    }});
  </script>
</body>
</html>";

            string parentHtml = $@"<!DOCTYPE html>
<html>
<head>
  <meta charset=""utf-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
  <style>
    body {{ font-size: 24px; padding: 20px; }}
    #status {{ color:#888; font-size:14px; margin-bottom:8px; }}
    iframe {{ width: 100%; border: 0; min-height: 60px; }}
  </style>
</head>
<body>
  <div id=""status"">Loading math…</div>
  <iframe id=""mathframe""></iframe>
  <script>
    const src = {JsonSerializer.Serialize(iframeHtml)};
    const frame = document.getElementById('mathframe');
    const status = document.getElementById('status');
    frame.addEventListener('load', function () {{
      status.remove();
      setTimeout(function() {{
        const doc = frame.contentDocument || frame.contentWindow.document;
        frame.style.height = (doc && doc.body ? doc.body.scrollHeight : 0) + 'px';
      }}, 50);
    }});
    frame.srcdoc = src;
  </script>
</body>
</html>";

            return new HtmlWebViewSource { Html = parentHtml };
        }

        private static bool LooksLikeLatex(string reply)
        {
            if (string.IsNullOrWhiteSpace(reply)) return false;

            return reply.Contains("\\frac") || reply.Contains("\\sum") ||
                   reply.Contains("\\int") || reply.Contains("\\sqrt") ||
                   reply.Contains("\\begin") || reply.Contains("\\end") ||
                   reply.Contains("$") || reply.Contains("\\[") || reply.Contains("\\(");
        }

        private static string ConvertReplyToLatex(string reply)
        {
            // The check is now done externally, so we just process.
            // If it's not LaTeX, we still escape it to be safe, though this path is less likely to be hit.
            if (LooksLikeLatex(reply))
            {
                return reply;
            }

            string escaped = EscapeForLatexText(reply);
            return $"\\text{{{escaped}}}";
        }

        private static string EscapeForLatexText(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            StringBuilder sb = new();
            foreach (char c in input)
            {
                switch (c)
                {
                    case '\\': sb.Append("\\textbackslash{}"); break;
                    case '{': sb.Append("\\{"); break;
                    case '}': sb.Append("\\}"); break;
                    case '$': sb.Append("\\$"); break;
                    case '#': sb.Append("\\#"); break;
                    case '%': sb.Append("\\%"); break;
                    case '&': sb.Append("\\&"); break;
                    case '_': sb.Append("\\_"); break;
                    case '^': sb.Append("\\^"); break;
                    case '~': sb.Append("\\textasciitilde{}"); break;
                    case '<': sb.Append("\\textless{}"); break;
                    case '>': sb.Append("\\textgreater{}"); break;
                    default: sb.Append(c); break;
                }
            }
            return sb.ToString();
        }
    }
}
