namespace CultureStay.Application.ViewModels.User.Request;

public class UpdateUserInfoRequest
{
    public string FullName { get; set; } = null!;
    public string? Introduction { get; set; }
    public string? PhoneNumber { get; set; }
    public string Email { get; set; } = null!;
    public string? City { get; set; }
    public string? Address { get; set; }
    public string? AvatarUrl { get; set; }
}