using CultureStay.Domain.Entities.Base;
using CultureStay.Domain.Enum;

namespace CultureStay.Domain.Entities;

public class ChargePayment  :EntityBase
{
    public string PaymentCode { get; set; } = null!;
    public string BookingPaymentCode { get; set; } = null!;
    
    public int HostId { get; set; }
    public Host Host { get; set; } = null!;
    
    public int CancellationTicketId { get; set; }
    public CancellationTicket CancellationTicket { get; set; } = null!;
    
    public double Amount { get; set; }
    public ChargePaymentStatus Status { get; set; }
}
