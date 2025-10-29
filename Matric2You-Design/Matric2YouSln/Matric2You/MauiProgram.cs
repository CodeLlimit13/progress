using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging;
using Matric2You.Services;
using Matric2You.ViewModel;
using Matric2You.Views;
 
namespace Matric2You
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton(provider =>
            {
                const bool USE_HARDCODED_VALUES = true; // Set false to use environment variables

                string? endpoint;
                string? deploymentName;
                string? searchEndpoint;
                string? searchIndex;
                string? searchApiKey;
                string? openAiApiKey;

                if (USE_HARDCODED_VALUES)
                {
                    endpoint = "https://sparkchatmodel.openai.azure.com/";
                    deploymentName = "gpt-4";
                    searchEndpoint = "https://sparksearchservice.search.windows.net";
                    searchIndex = "sparkrag";
                    searchApiKey = "P0NXcq6Ptj483FiIBtxfVWsuuUdyAIzXhYivIDEdq0AzSeAirl9R";
                    openAiApiKey = "9bHLx9uikYj7WO0bqwGXiHOTFQiqYAa1jTwUiXeMvQBn6bEKfRlaJQQJ99BIACYeBjFXJ3w3AAABACOGbmaN";
                }
                else
                {
                    endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
                    deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_ID");
                    searchEndpoint = Environment.GetEnvironmentVariable("AZURE_AI_SEARCH_ENDPOINT");
                    searchIndex = Environment.GetEnvironmentVariable("AZURE_AI_SEARCH_INDEX");
                    searchApiKey = Environment.GetEnvironmentVariable("AZURE_AI_SEARCH_API_KEY");
                    openAiApiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
                }

                return new SparkChatService(
                    endpoint!,
                    deploymentName!,
                    searchEndpoint!,
                    searchIndex!,
                    searchApiKey,
                    openAiApiKey);
            });

            builder.Services.AddSingleton<ChatViewModel>();
            builder.Services.AddSingleton<ChatBotPage>();

            return builder.Build();
        }
    }
}
