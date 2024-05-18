using Newtonsoft.Json;

namespace CultureStay.Application.ViewModels.Review.Response;

public class CreateReviewRequest
{
    [JsonIgnore]
    public int ReviewerId { get; set; }
    [JsonIgnore]
    public int RevieweeId { get; set; }
    public int Rating { get; set; }
    public string Content { get; set; } = string.Empty;
}