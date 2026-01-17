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

        System.Diagnostics.Debug.WriteLine($"[PRListPage] OnAppearing - hasInitialized: {_hasInitialized}, PRCount: {_viewModel.PullRequests.Count}");

        // 常に InitializeAsync を呼ぶ（認証状態が変わった可能性があるため）
        await _viewModel.InitializeAsync();
        _hasInitialized = true;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.StopAutoRefresh();
    }

    private void OnToggleSidebarClicked(object sender, EventArgs e)
    {
        _isSidebarVisible = !_isSidebarVisible;
        Sidebar.IsVisible = _isSidebarVisible;

        ToggleSidebarButton.Text = _isSidebarVisible ? "☰" : "☰";
    }
}

