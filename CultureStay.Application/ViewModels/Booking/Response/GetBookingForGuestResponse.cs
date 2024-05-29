using System.Text.Json.Serialization;
using CultureStay.Domain.Enum;

namespace CultureStay.Application.ViewModels.Booking.Response;

public class GetBookingForGuestResponse
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public string PropertyName { get; set; } = null!;
    public int HostId { get; set; }
    public string HostName { get; set; } = null!;
    public string HostEmail { get; set; } = null!;
    public string HostPhoneNumber { get; set; } = null!;
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BookingStatus Status { get; set; }
    
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfDays { get; set; }
    
    public int NumberOfGuest { get; set; }
    public string? Note { get; set; }
    public string? CheckInCode { get; set; }
}