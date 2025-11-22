using PullRequestReviewer.ViewModels;

namespace PullRequestReviewer.Views;

public partial class PRListPage : ContentPage
{
    private readonly PRListViewModel _viewModel;
    private bool _isSidebarVisible = true;
    private bool _hasInitialized = false;

    public PRListPage(PRListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Only initialize once, or when returning from settings page
        if (!_hasInitialized || _viewModel.PullRequests.Count == 0)
        {
            await _viewModel.InitializeAsync();
            _hasInitialized = true;
        }
    }

    private void OnToggleSidebarClicked(object sender, EventArgs e)
    {
        _isSidebarVisible = !_isSidebarVisible;
        Sidebar.IsVisible = _isSidebarVisible;

        ToggleSidebarButton.Text = _isSidebarVisible ? "☰" : "☰";
    }
}
