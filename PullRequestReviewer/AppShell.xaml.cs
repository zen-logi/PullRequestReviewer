using PullRequestReviewer.Views;

namespace PullRequestReviewer;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("TokenSettingPage", typeof(TokenSettingPage));
    }
}
