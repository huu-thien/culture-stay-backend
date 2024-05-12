using AutoMapper;
using CultureStay.Domain.Entities;

namespace CultureStay.Application.ViewModels.Review.Response;

public class GetPropertyReviewResponse
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public int GuestId { get; set; }
    public int UserId { get; set; }
    public string? GuestName { get; set; } = string.Empty;
    public string? GuestAvatarUrl { get; set; } = string.Empty;
    
    public int Cleanliness { get; set; }
    public int Communication { get; set; }
    public int CheckIn { get; set; }
    public int Accuracy { get; set; }
    public int Location { get; set; }
    public int Value { get; set; }
    
    public double AverageRating { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime ReviewTime { get; set; }
}
