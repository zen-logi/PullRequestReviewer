namespace PullRequestReviewer;

public partial class App : Application
{
    private readonly AppShell _appShell;

    public App(AppShell appShell)
    {
        InitializeComponent();
        _appShell = appShell;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(_appShell);

        // Restore window size and position
        if (Preferences.ContainsKey("WindowX") && Preferences.ContainsKey("WindowY") &&
            Preferences.ContainsKey("WindowWidth") && Preferences.ContainsKey("WindowHeight"))
        {
            var x = Preferences.Get("WindowX", 0.0);
            var y = Preferences.Get("WindowY", 0.0);
            var width = Preferences.Get("WindowWidth", 0.0);
            var height = Preferences.Get("WindowHeight", 0.0);

            window.X = x;
            window.Y = y;
            window.Width = width;
            window.Height = height;
        }

        window.Destroying += (s, e) =>
        {
            Preferences.Set("WindowX", window.X);
            Preferences.Set("WindowY", window.Y);
            Preferences.Set("WindowWidth", window.Width);
            Preferences.Set("WindowHeight", window.Height);
        };

        return window;
    }
}
