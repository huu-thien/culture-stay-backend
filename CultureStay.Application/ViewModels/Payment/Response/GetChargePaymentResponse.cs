using CultureStay.Domain.Enum;

namespace CultureStay.Application.ViewModels.Payment.Response;

public class GetChargePaymentResponse
{
    
    public string PaymentCode { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; }
    public double Amount { get; set; }
    public DateTime CreateAt { get; set; }
    public ChargePaymentStatus Status { get; set; }
}