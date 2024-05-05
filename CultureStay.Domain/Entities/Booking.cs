using CultureStay.Domain.Entities.Base;
using CultureStay.Domain.Enum;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CultureStay.Domain.Entities;

public class Booking : EntityBase
{
    public int PropertyId { get; set; }
    public Property Property { get; set; } = null!;
    public int GuestId { get; set; }
    public Guest Guest { get; set; } = null!;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfGuests { get; set; }
    public string? Note { get; set; }
    public BookingStatus Status { get; set; }
    
    public int? CancellationTicketId { get; set; }
    public CancellationTicket? CancellationTicket { get; set; }
    public string? Guid { get; set; }
    
    // public double CleaningFee { get; set; }
    // public double PricePerNight { get; set; }
    // public double SystemFee { get; set; }
    // public double TotalPrice { get; set; }
    // public CancellationPolicyType CancellationPolicyType { get; set; }
}