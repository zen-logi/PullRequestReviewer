# Pull Request Reviewer

A cross-platform application for managing and reviewing your GitHub pull requests.

## Features

- View all pull requests assigned to you, requesting your review, or authored by you
- Filter pull requests by type (All, Review Requested, Assigned, Authored)
- Open pull requests directly in your browser
- Cross-platform support (Windows, macOS, iOS, Android)

## Installation

### Windows

1. Download the latest release from the [Releases](../../releases) page
2. Extract the zip file
3. Run `PullRequestReviewer.exe`

### macOS

1. Download the latest release from the [Releases](../../releases) page
2. Open the `.dmg` file
3. Drag the app to your Applications folder
4. Launch Pull Request Reviewer

### iOS

1. Download from the App Store (coming soon)

### Android

1. Download from Google Play Store (coming soon)

## Setup

### GitHub Token Configuration

On first launch, you'll be prompted to enter your GitHub Personal Access Token.

#### Creating a GitHub Token

1. Go to [GitHub Settings > Developer settings > Personal access tokens > Tokens (classic)](https://github.com/settings/tokens)
2. Click "Generate new token" > "Generate new token (classic)"
3. Give your token a descriptive name (e.g., "Pull Request Reviewer")
4. Select the following scopes:
   - `repo` - Full control of private repositories
   - `read:user` - Read user profile data
5. Click "Generate token"
6. Copy the token (you won't be able to see it again)
7. Paste it into the Pull Request Reviewer app when prompted

#### Updating Your Token

To change your GitHub token later:
1. Open the app
2. Click the "Settings" button in the toolbar
3. Enter your new token
4. Click "Save"

## Usage

1. Launch the application
2. Enter your GitHub token (first time only)
3. The app will automatically load your pull requests
4. Use the sidebar to filter by type:
   - **All**: All pull requests across all categories
   - **Review Requested**: Pull requests where your review has been requested
   - **Assigned**: Pull requests assigned to you
   - **Authored**: Pull requests you created
5. Click on any pull request to open it in your browser
6. Pull down to refresh the list

## Troubleshooting

### No pull requests displayed

- Verify your GitHub token has the correct permissions (`repo` and `read:user`)
- Check your internet connection
- Try refreshing the list by pulling down

### Token validation failed

- Ensure your token hasn't expired
- Verify you copied the entire token without extra spaces
- Generate a new token if needed

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

If you encounter any issues or have suggestions, please [open an issue](../../issues) on GitHub.