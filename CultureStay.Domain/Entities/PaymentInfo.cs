using CultureStay.Domain.Entities.Base;

namespace CultureStay.Domain.Entities;

public class PaymentInfo : EntityBase
{
    public int HostId { get; set; }
    public Host Host { get; set; } = null!;
    
    public string BankName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountHolder { get; set; } = string.Empty;
    
    public ICollection<HostPayment> HostPayments { get; set; } = null!;
}