# Pull Request Reviewer

A cross-platform application for managing and reviewing your GitHub pull requests.
<img width="993" height="341" alt="image" src="https://github.com/user-attachments/assets/e5015f77-1718-43b0-8f4f-7a3ac786211c" />

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

### Authentication Methods

Pull Request Reviewer supports two authentication methods:

| Method | Best For | Notes |
|--------|----------|-------|
| **GitHub Login (OAuth)** | Personal repositories | Simple one-click login |
| **Personal Access Token** | Organization repositories | No organization approval needed |

---

### Option 1: GitHub Login (OAuth)

1. Launch the app
2. Click **"Login with GitHub"**
3. A browser window will open
4. Enter the displayed code at `github.com/login/device`
5. Authorize the app

> ⚠️ **Organization Repositories**: If your PRs are in organization repositories, the organization admin must approve the OAuth app. Go to GitHub > Settings > Applications > Authorized OAuth Apps > "PullRequestReviewer" > Grant access to your organization.

---

### Option 2: Personal Access Token (Recommended for Organizations)

#### Creating a GitHub Token

1. Go to [GitHub Settings > Developer settings > Personal access tokens](https://github.com/settings/tokens)
2. Click "Generate new token" > "Generate new token (classic)"
3. Give your token a descriptive name (e.g., "Pull Request Reviewer")
4. Select the `repo` scope
5. Click "Generate token"
6. Copy the token and paste it into the app

#### Using the Token

1. Launch the app
2. Click **"Use Personal Token"**
3. Paste your token and click **"Save Token"**

---

### Changing Authentication

To switch authentication methods or logout:
1. Open Settings
2. Click "Logout" (if logged in via OAuth)
3. Choose your preferred authentication method

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

- **Using OAuth with organization repos?** The organization must approve the app (see Setup section)
- Verify your GitHub token has the `repo` permission
- Check your internet connection
- Try refreshing the list

### Token validation failed

- Ensure your token hasn't expired
- Verify you copied the entire token without extra spaces
- Generate a new token if needed

### OAuth login not working

- Make sure you entered the correct code at `github.com/login/device`
- Try logging out and logging in again
- For organization repositories, use Personal Access Token instead

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

If you encounter any issues or have suggestions, please [open an issue](../../issues) on GitHub.
