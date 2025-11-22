namespace PullRequestReviewer;

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
            })
            .ConfigureMauiHandlers(handlers =>
            {
#if WINDOWS
                // Windows specific handlers can be added here
#endif
            });

        // Load configuration
        var config = Startup.LoadConfiguration();

        // Configure services and logging
        var startup = new Startup();
        startup.Configure(builder.Logging, config);
        startup.ConfigureServices(builder.Services, config);

        return builder.Build();
    }
}
