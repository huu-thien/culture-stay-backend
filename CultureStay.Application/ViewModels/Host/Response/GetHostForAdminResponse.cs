namespace CultureStay.Application.ViewModels.Host.Response;

public class GetHostForAdminResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public string? City { get; set; }
    public DateTime JoinedAt { get; set; }
    public int NumberOfReviews { get; set; }
    public double Rating { get; set; }
    public bool IsBlocked { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public int NumberOfProperties { get; set; }
    public int NumberOfBookings { get; set; }
    public double TotalIncome { get; set; }
}