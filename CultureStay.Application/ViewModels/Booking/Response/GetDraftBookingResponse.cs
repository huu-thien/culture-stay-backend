namespace CultureStay.Application.ViewModels.Booking.Response;

public class GetDraftBookingResponse
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public string PropertyName { get; set; } = null!;
    
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfDays { get; set; }
    public int NumberOfGuest { get; set; }
    public string? Note { get; set; }
}