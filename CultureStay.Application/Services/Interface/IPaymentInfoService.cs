using CultureStay.Application.ViewModels.PaymentInfo.Response;

namespace CultureStay.Application.Services.Interface;

public interface IPaymentInfoService
{
    Task<PaymentInfoResponse> GetPaymentInfoAsync();
    Task<PaymentInfoResponse> CreatePaymentInfoAsync(PaymentInfoResponse paymentInfoDto);
    Task<PaymentInfoResponse> UpdatePaymentInfoAsync(PaymentInfoResponse paymentInfoDto);
}