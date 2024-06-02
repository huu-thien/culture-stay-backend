namespace CultureStay.Application.ViewModels.Chat.Response;

public class GetContactResponse
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; } = string.Empty;
    public string? LastMessage { get; set; } = string.Empty;
    public DateTime? LastMessageTime { get; set; }
}