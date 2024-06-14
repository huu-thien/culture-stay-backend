using CultureStay.Domain.Entities.Base;
using CultureStay.Domain.Enum;

namespace CultureStay.Domain.Entities;

public class BookingPayment : EntityBase
{
    public string PaymentCode { get; set; } = null!;
    
    public int GuestId { get; set; }
    public Guest Guest { get; set; } = null!;
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    public double Amount { get; set; }
    public BookingPaymentStatus Status { get; set; } = BookingPaymentStatus.Pending;
    
    public string? Description { get; set; }
    public ICollection<VnpHistory> VnpHistories { get; set; } = new List<VnpHistory>();
}