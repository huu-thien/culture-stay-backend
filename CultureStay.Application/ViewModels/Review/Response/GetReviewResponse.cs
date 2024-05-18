namespace CultureStay.Application.ViewModels.Review.Response;

public class GetReviewResponse
{
    public int Id { get; set; }
    public int ReviewerId { get; set; }
    public int UserId { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
    public string? ReviewerAvatarUrl { get; set; } = string.Empty;
    public DateTime ReviewTime { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; }
}