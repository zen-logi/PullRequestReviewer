namespace PullRequestReviewer.Models;

public class PullRequestModel
{
    public long Id { get; set; }
    public int Number { get; set; }
    public string? Title { get; set; }
    public string? State { get; set; }
    public string Body { get; set; } = string.Empty;
    public string? HtmlUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string AuthorLogin { get; set; } = string.Empty;
    public string? AuthorAvatarUrl { get; set; }
    public string RepositoryName { get; set; } = string.Empty;
    public string RepositoryOwner { get; set; } = string.Empty;
    public string RepositoryFullName { get; set; } = string.Empty;
    public bool IsDraft { get; set; }

    public List<string> RequestedReviewers { get; set; } = null!;
    public List<string> Assignees { get; set; } = null!;

    public string DisplayCreatedAt => CreatedAt.ToLocalTime().ToString("yyyy/MM/dd HH:mm");
    public string StateColor => State?.ToLower() == "open" ? "#28a745" : "#6f42c1";
}
