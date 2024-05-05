using CultureStay.Domain.Entities.Base;

namespace CultureStay.Domain.Entities;

public class Message : EntityBase
{
    public int SenderId { get; set; }
    public string? SenderAvatarUrl { get; set; }
    public string SenderName { get; set; } = null!;
    public int ReceiverId { get; set; }
    public string Content { get; set; } = null!;
    
    public User Sender { get; set; } = null!;
    public User Receiver { get; set; } = null!;
}