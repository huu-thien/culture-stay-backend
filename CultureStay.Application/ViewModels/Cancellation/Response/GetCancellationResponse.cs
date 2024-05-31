using System.Text.Json.Serialization;
using CultureStay.Domain.Enum;

namespace CultureStay.Application.ViewModels.Cancellation.Response;

public class GetCancellationResponse
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    
    public int IssuerId { get; set; }
    public int TheOtherPartyId { get; set; }
    public bool IsIssuerGuest { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CancellationReason CancellationReason { get; set; }
    public string? CancellationReasonNote { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CancellationTicketStatus Status { get; set; }
    
    public string? ResolveNote { get; set; }
    public List<string> Attachments { get; set; } = new();
}