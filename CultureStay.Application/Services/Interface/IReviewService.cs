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
    Task<GetPropertyReviewResponse> UpdatePropertyReviewAsync(int propertyId, int reviewId, CreatePropertyReviewRequest createPropertyReviewRequest);
    Task DeletePropertyReviewAsync(int propertyReviewId);
    
    // Guest Review
    // Host Review

}