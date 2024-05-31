using CultureStay.Application.Common.Models;
using CultureStay.Application.ViewModels.Cancellation.QueryParameters;
using CultureStay.Application.ViewModels.Cancellation.Request;
using CultureStay.Application.ViewModels.Cancellation.Response;

namespace CultureStay.Application.Services.Interface;

public interface ICancellationService
{
    Task<GetCancellationResponse> CreateCancellationTicketAsync(CreateCancellationRequest request);
    
    Task<PaginatedList<GetCancellationResponse>> GetCancellationTicketAsync(CancellationTicketQueryParameters ctp);
    
    Task<GetCancellationResponse> GetCancellationTicketByIdAsync(int id);
    
    Task RejectCancellationTicketAsync(int cancellationTicketId, RejectCancellationTicketRequest request);
    
    Task AcceptCancellationTicketAsync(int cancellationTicketId, string? resolveNote);
}