using System.Text.Json.Serialization;
using CultureStay.Domain.Enum;

namespace CultureStay.Application.ViewModels.Cancellation.Request;

public class CreateCancellationRequest
{
    public int BookingId { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CancellationReason CancellationReason { get; set; } = CancellationReason.Other;
    
    public string? Reason { get; set; }
    
    public bool IsGuest { get; set; }
    
    public List<string> Attachments { get; set; } = new();
}