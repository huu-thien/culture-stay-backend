using CultureStay.Application.Common.Models;
using CultureStay.Application.ViewModels.Payment.QueryParameters;
using CultureStay.Application.ViewModels.Payment.Request;
using CultureStay.Application.ViewModels.Payment.Response;

namespace CultureStay.Application.Services.Interface;

public interface IPaymentService
{
    Task<PaginatedList<GetChargePaymentResponse>> ChargePayment(ChargePaymentQueryParameter pqp);
    Task<string> CreateBookingPayment(string ip, CreateBookingPaymentRequest createBookingPaymentDto);
    Task ReceiveDataFromVnp(VnPayReturnDto vnpayReturnDto);
    Task<PaginatedList<GetRefundPaymentResponse>> RefundPayment(RefundPaymentQueryParameter pqp);
}