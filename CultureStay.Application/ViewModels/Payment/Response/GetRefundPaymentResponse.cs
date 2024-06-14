using CultureStay.Domain.Enum;

namespace CultureStay.Application.ViewModels.Payment.Response;

public class GetRefundPaymentResponse
{
    public string PaymentCode { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; }
    public double Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public RefundPaymentStatus Status { get; set; }
}