using System.Text.Json.Serialization;
using CultureStay.Domain.Enum;

namespace CultureStay.Application.ViewModels.Booking.Request;

public class UpdateStatusBookingRequest
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BookingStatus Status { get; set; }
}