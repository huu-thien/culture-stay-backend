namespace CultureStay.Application.ViewModels.Auth.Responses;

public class GetUserResponse
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public bool IsHost { get; set; }
}