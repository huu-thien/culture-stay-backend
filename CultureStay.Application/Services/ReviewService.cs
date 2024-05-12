using AutoMapper;
using CultureStay.Application.Common.Interfaces;
using CultureStay.Application.Common.Models;
using CultureStay.Application.Common.Specifications;
using CultureStay.Application.Services.Interface;
using CultureStay.Application.ViewModels.Review.QueryParameters;
using CultureStay.Application.ViewModels.Review.Response;
using CultureStay.Application.ViewModels.Review.Specifications;
using CultureStay.Domain.Constants;
using CultureStay.Domain.Entities;
using CultureStay.Domain.Exceptions;
using CultureStay.Domain.Repositories.Base;

namespace CultureStay.Application.Services;

public class ReviewService (
    IRepositoryBase<Property> propertyRepository,
    IRepositoryBase<PropertyReview> propertyReviewRepository,
    IRepositoryBase<Guest> guestRepository,
    IRepositoryBase<GuestReview> guestReviewRepository,
    IRepositoryBase<Host> hostRepository,
    IRepositoryBase<HostReview> hostReviewRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentUser currentUser
        
        ) :  BaseService(unitOfWork, mapper, currentUser), IReviewService
{
    // Property Review
    public async Task<PaginatedList<GetPropertyReviewResponse>> GetListPropertyReviewAsync(int propertyId, ReviewQueryParameters reviewQueryParameters)
    {
        var (propertyReviews, totalCount) = await propertyReviewRepository
                                    .FindWithTotalCountAsync(new PropertyReviewSpecification(propertyId, reviewQueryParameters));
        var mappingPropertyReviews = Mapper.Map<List<GetPropertyReviewResponse>>(propertyReviews);
        return new PaginatedList<GetPropertyReviewResponse>(mappingPropertyReviews, totalCount, 
            reviewQueryParameters.PageIndex, reviewQueryParameters.PageSize);
    }

    public async Task<GetPropertyReviewResponse> CreatePropertyReviewAsync(int propertyId, CreatePropertyReviewRequest request)
    {
        request.PropertyId = propertyId;
        var guest = await guestRepository.FindOneAsync(new GuestByUserIdSpecification(int.Parse(currentUser.Id!)));
        request.GuestId =
            guest?.Id ?? throw new EntityNotFoundException(nameof(Guest), propertyId.ToString());
        
        // Check if the guest has already reviewed the property
        var isPropertyExist = await propertyRepository.AnyAsync(propertyId);
        if (!isPropertyExist)
            throw new EntityNotFoundException(nameof(Property), propertyId.ToString());
        
        var isGuestExist = await guestRepository.AnyAsync(request.GuestId);
        if (!isGuestExist)
            throw new EntityNotFoundException(nameof(Guest), request.GuestId.ToString());
        
        var propertyReview = await propertyReviewRepository
            .FindOneAsync(new PropertyReviewByIdsSpecification(propertyId, request.GuestId));
        
        // Update the existing review or create new one
        if (propertyReview != null) Mapper.Map<PropertyReview>(request);
        else
        {
            propertyReview = Mapper.Map<PropertyReview>(request);
            propertyReviewRepository.Add(propertyReview);
        }

        await unitOfWork.SaveChangesAsync();
        var reviewResponse = Mapper.Map<GetPropertyReviewResponse>(propertyReview);
        reviewResponse.GuestName = currentUser.Name;
        reviewResponse.GuestAvatarUrl = currentUser.AvatarUrl;
        return reviewResponse;
        
    }

    public Task<GetPropertyReviewResponse> UpdatePropertyReviewAsync(int propertyId, int reviewId, CreatePropertyReviewRequest createPropertyReviewRequest)
    {
        throw new NotImplementedException();
    }

    public async Task DeletePropertyReviewAsync(int propertyReviewId)
    {
        var propertyReview = await propertyReviewRepository.GetByIdAsync(propertyReviewId)
            ?? throw new EntityNotFoundException(nameof(PropertyReview), propertyReviewId.ToString());
        
        // Check user have permission to delete the review
        var currentUserId = int.Parse(currentUser.Id!);
        var currentUserRole = currentUser.Role;

        if (currentUserRole != AppRole.Admin)
        {
            if(propertyReview.GuestId != currentUserId)
                throw new UnauthorizedAccessException("You do not have permission to delete this review");
        }
        propertyReviewRepository.Delete(propertyReview);
        await unitOfWork.SaveChangesAsync();
    }
    
    // Guest Review
    
    // Host Review
}


public class ReviewMapping : Profile
{
    public ReviewMapping()
    {
        CreateMap<PropertyReview, GetPropertyReviewResponse>()
            .ForMember(dto => dto.GuestName, opt => opt.MapFrom(hr => hr.Guest.User.FullName))
            .ForMember(dto => dto.UserId, opt => opt.MapFrom(hr => hr.Guest.UserId))
            .ForMember(dto => dto.GuestAvatarUrl, opt => opt.MapFrom(hr => hr.Guest.User.AvatarUrl))
            .ForMember(dto => dto.ReviewTime, opt => opt.MapFrom(hr => hr.LastModifiedAt))
            .ForMember(dto => dto.AverageRating, opt => opt
                .MapFrom(hr => ((double)(hr.Cleanliness + hr.Communication + hr.CheckIn + hr.Accuracy + hr.Location + hr.Value)) / 6));
    }
}