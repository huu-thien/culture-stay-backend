using CultureStay.Application.Common.Models;
using CultureStay.Application.ViewModels.Review.QueryParameters;
using CultureStay.Application.ViewModels.Review.Response;

namespace CultureStay.Application.Services.Interface;

public interface IReviewService
{
    // Property Review
    Task<PaginatedList<GetPropertyReviewResponse>> GetListPropertyReviewAsync(int propertyId,
        ReviewQueryParameters reviewQueryParameters);
    Task<GetPropertyReviewResponse> CreatePropertyReviewAsync(int propertyId, CreatePropertyReviewRequest createPropertyReviewRequest);
    Task DeletePropertyReviewAsync(int propertyReviewId);
    
    // Host Review
    Task<PaginatedList<GetReviewResponse>> GetHostReviewsAsync(int hostId, ReviewQueryParameters reviewQueryParameters);
    Task<GetReviewResponse> CreateHostReviewAsync(int hostId, CreateReviewRequest createReviewRequest);
    Task DeleteHostReviewAsync(int hostReviewId);
    
    // Guest Review
    Task<PaginatedList<GetReviewResponse>> GetGuestReviewsAsync(int guestId, ReviewQueryParameters reviewQueryParameters);
    Task<GetReviewResponse> CreateGuestReviewAsync(int guestId, CreateReviewRequest createReviewRequest);
    Task DeleteGuestReviewAsync(int guestReviewId);

}