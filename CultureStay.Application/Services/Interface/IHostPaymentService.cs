using CultureStay.Application.Common.Models;
using CultureStay.Application.ViewModels.HostPayment.QueryParameters;
using CultureStay.Application.ViewModels.HostPayment.Response;

namespace CultureStay.Application.Services.Interface;

public interface IHostPaymentService
{
    Task<PaginatedList<HostPaymentResponse>> GetPaymentsAsync(HostPaymentQueryParameters hqp);
    Task<HostPaymentResponse> GetPaymentAsync(int hostPaymentId);
    Task Pay(int hostPaymentId);
    Task<PaginatedList<HostPaymentResponse>> GetPaymentsByHostIdAsync(int hostId, HostPaymentQueryParameters hqp);
}

