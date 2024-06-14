using System.Text.Json.Serialization;
using CultureStay.Domain.Enum;

namespace CultureStay.Application.ViewModels.HostPayment.Response;

public class HostPaymentResponse
{
    public int Id { get; set; }
    public HostPaymentInfoResponse PaymentInfo { get; set; } = null!;

    public int BookingId { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public HostPaymentStatus Status { get; set; }

    public double Amount { get; set; }

    public string? Description { get; set; }
}