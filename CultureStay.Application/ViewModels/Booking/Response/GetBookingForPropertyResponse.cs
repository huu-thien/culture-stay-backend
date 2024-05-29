using CultureStay.Domain.Enum;

namespace CultureStay.Application.ViewModels.Booking.Response;

public class GetBookingForPropertyResponse
{
    public int Id { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public BookingStatus Status { get; set; }
}