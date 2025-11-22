using PullRequestReviewer.ViewModels;

namespace PullRequestReviewer.Views;

public partial class TokenSettingPage : ContentPage
{
    private readonly TokenSettingViewModel _viewModel;

    public TokenSettingPage(TokenSettingViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}
