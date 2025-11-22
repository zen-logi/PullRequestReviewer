using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PullRequestReviewer.Services;
using PullRequestReviewer.ViewModels;
using PullRequestReviewer.Views;

namespace PullRequestReviewer;

public class Startup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Configure Options
        services.Configure<Models.Options.GitHubOptions>(configuration.GetSection(Models.Options.GitHubOptions.SectionName));

        // Register Services with HttpClient
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddHttpClient<IGitHubService, GitHubService>()
            .ConfigureHttpClient((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<Models.Options.GitHubOptions>>().Value;
                client.Timeout = TimeSpan.FromSeconds(options.RequestTimeout);
            });

        // Register ViewModels
        services.AddTransient<TokenSettingViewModel>();
        services.AddTransient<PRListViewModel>();

        // Register Views
        services.AddTransient<TokenSettingPage>();
        services.AddTransient<PRListPage>();

        // Register AppShell
        services.AddSingleton<AppShell>();
    }

    public void Configure(ILoggingBuilder logging, IConfiguration configuration)
    {
        // Configure logging
        logging.AddConfiguration(configuration.GetSection("Logging"));
#if DEBUG
        logging.AddDebug();
        logging.AddConsole();
#endif
    }

    public static IConfiguration LoadConfiguration()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var baseStream = assembly.GetManifestResourceStream("PullRequestReviewer.appsettings.json");

        var configBuilder = new ConfigurationBuilder();
        if (baseStream != null)
        {
            configBuilder.AddJsonStream(baseStream);
        }

        // Load environment-specific configuration
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
        var envConfigName = $"PullRequestReviewer.appsettings.{environment}.json";
        using var envStream = assembly.GetManifestResourceStream(envConfigName);
        if (envStream != null)
        {
            configBuilder.AddJsonStream(envStream);
        }

        return configBuilder.Build();
    }
}
