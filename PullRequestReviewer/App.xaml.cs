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

        // Restore window size and position with screen boundary validation
        if (Preferences.ContainsKey("WindowX") && Preferences.ContainsKey("WindowY") &&
            Preferences.ContainsKey("WindowWidth") && Preferences.ContainsKey("WindowHeight"))
        {
            var x = Preferences.Get("WindowX", 0.0);
            var y = Preferences.Get("WindowY", 0.0);
            var width = Preferences.Get("WindowWidth", 0.0);
            var height = Preferences.Get("WindowHeight", 0.0);

            // Validate window position is within screen bounds
            if (IsWindowPositionValid(x, y, width, height))
            {
                window.X = x;
                window.Y = y;
                window.Width = width;
                window.Height = height;
            }
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

    /// <summary>
    /// Validates that the window position and size are within the current display bounds.
    /// </summary>
    /// <param name="x">Window X position</param>
    /// <param name="y">Window Y position</param>
    /// <param name="width">Window width</param>
    /// <param name="height">Window height</param>
    /// <returns>True if position is valid, false otherwise</returns>
    private static bool IsWindowPositionValid(double x, double y, double width, double height)
    {
        // Ensure minimum valid dimensions
        const double minWidth = 100;
        const double minHeight = 100;

        if (width < minWidth || height < minHeight)
        {
            return false;
        }

        try
        {
            var displayInfo = DeviceDisplay.MainDisplayInfo;

            // Convert display dimensions to DIPs (density-independent pixels)
            var screenWidth = displayInfo.Width / displayInfo.Density;
            var screenHeight = displayInfo.Height / displayInfo.Density;

            // Check if at least a portion of the window is visible on screen
            // Allow some tolerance for window to be partially off-screen
            const double minVisiblePortion = 50;

            var isXValid = x + width > minVisiblePortion && x < screenWidth - minVisiblePortion;
            var isYValid = y + height > minVisiblePortion && y < screenHeight - minVisiblePortion;

            return isXValid && isYValid;
        }
        catch
        {
            // If we can't get display info, don't restore position
            return false;
        }
    }
}
