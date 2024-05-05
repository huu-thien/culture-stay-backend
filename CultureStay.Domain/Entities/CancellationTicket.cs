using CultureStay.Domain.Entities;
using CultureStay.Domain.Entities.Base;

namespace CultureStay.Domain.Enum;

public class CancellationTicket: EntityBase
{
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    
    public int TheOtherPartyId { get; set; }
    
    public bool IsIssuerGuest { get; set; }
    
    public CancellationReason CancellationReason { get; set; }
    
    public string? CancellationReasonNote { get; set; }
    
    public CancellationTicketStatus Status { get; set; }
    public string? ResolveNote { get; set; }
    
    public ICollection<CancellationTicketAttachment> Attachments { get; set; } = null!;
}
