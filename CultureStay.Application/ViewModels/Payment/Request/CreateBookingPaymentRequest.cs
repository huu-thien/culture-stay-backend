namespace CultureStay.Application.ViewModels.Payment.Request;

public class CreateBookingPaymentRequest
{
    public int BookingId { get; set; }
    public string? BankCode { get; set; } = "VNBANK";
}