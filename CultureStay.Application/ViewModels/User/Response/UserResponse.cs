namespace CultureStay.Application.ViewModels.User.Response;

public class UserResponse
{
    public string Id { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? AvatarUrl { get; set; }
}