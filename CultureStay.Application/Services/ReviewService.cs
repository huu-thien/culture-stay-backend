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

    
        
    // Host Review
    public async Task<PaginatedList<GetReviewResponse>> GetHostReviewsAsync(int hostId, ReviewQueryParameters reviewQueryParameters)
    {
        var (hostReviews, totalCount) = await hostReviewRepository
            .FindWithTotalCountAsync(new HostReviewSpecification(hostId, reviewQueryParameters));
        
        var mappingHostReviews = Mapper.Map<List<GetReviewResponse>>(hostReviews);
        return new PaginatedList<GetReviewResponse>(mappingHostReviews, totalCount, 
            reviewQueryParameters.PageIndex, reviewQueryParameters.PageSize);
    }

    public async Task<GetReviewResponse> CreateHostReviewAsync(int hostId, CreateReviewRequest createReviewRequest)
    {
        createReviewRequest.RevieweeId = hostId;
        var reviewer = await guestRepository.FindOneAsync(new GuestByUserIdSpecification(int.Parse(currentUser.Id!)));
        createReviewRequest.ReviewerId =
            reviewer?.Id ?? throw new EntityNotFoundException(nameof(Guest), currentUser.Id!);
        //  Check if Guest has ever booked this host
        var isHostExist = await hostRepository.AnyAsync(hostId);
        if (!isHostExist)
            throw new EntityNotFoundException(nameof(Host), hostId.ToString());
        var isGuestExist = await guestRepository.AnyAsync(createReviewRequest.ReviewerId);
        if (!isGuestExist)
            throw new EntityNotFoundException(nameof(Guest), createReviewRequest.ReviewerId.ToString());
        
        var hostReview = await hostReviewRepository.FindOneAsync(new HostReviewByIdsSpecification(hostId, createReviewRequest.ReviewerId));
        
        // Update the existing review or create new one
        if (hostReview != null) Mapper.Map(createReviewRequest, hostReview);
        else
        {
            hostReview = Mapper.Map<HostReview>(createReviewRequest);
            hostReviewRepository.Add(hostReview);
        }

        await unitOfWork.SaveChangesAsync();
        var reviewResponse = Mapper.Map<GetReviewResponse>(hostReview);
        reviewResponse.ReviewerName = currentUser.Name!;
        reviewResponse.ReviewerAvatarUrl = currentUser.AvatarUrl!;
        
        return reviewResponse;
    }

    public async Task DeleteHostReviewAsync(int hostReviewId)
    {
        var hostReview = await hostReviewRepository.GetByIdAsync(hostReviewId)
            ?? throw new EntityNotFoundException(nameof(HostReview), hostReviewId.ToString());
        
        // Check user have permission to delete the review
        var currentUserId = int.Parse(currentUser.Id!);
        var currentUserRole = currentUser.Role!;

        if (!string.Equals(currentUserRole, AppRole.Admin, StringComparison.CurrentCultureIgnoreCase))
        {
            var guest = await guestRepository.FindOneAsync(new GuestByUserIdSpecification(currentUserId));
            if(hostReview.GuestId != guest?.Id) 
                throw new ForbiddenAccessException("You do not have permission to delete this review");
        }
        hostReviewRepository.Delete(hostReview);
        await unitOfWork.SaveChangesAsync();
    }

    
    
    // Guest Review
    public async Task<PaginatedList<GetReviewResponse>> GetGuestReviewsAsync(int guestId, ReviewQueryParameters reviewQueryParameters)
    {
        var (guestReviews, totalCount) = await guestReviewRepository
            .FindWithTotalCountAsync(new GuestReviewSpecification(guestId, reviewQueryParameters));
        
        var mappingGuestReviews = Mapper.Map<List<GetReviewResponse>>(guestReviews);
        return new PaginatedList<GetReviewResponse>(mappingGuestReviews, totalCount, 
            reviewQueryParameters.PageIndex, reviewQueryParameters.PageSize);
    }

    public async Task<GetReviewResponse> CreateGuestReviewAsync(int guestId, CreateReviewRequest createReviewRequest)
    {
        createReviewRequest.RevieweeId = guestId;
        var reviewer = await hostRepository.FindOneAsync(new HostByUserIdSpecification(int.Parse(currentUser.Id!)));
        createReviewRequest.ReviewerId =
            reviewer?.Id ?? throw new EntityNotFoundException(nameof(Host), currentUser.Id!);
        //  Check if Host has ever hosted this guest
        var host = await hostRepository.FindOneAsync(new HostByUserIdSpecification(int.Parse(currentUser.Id!)));
        if(host is null) throw new EntityNotFoundException(nameof(Host), createReviewRequest.ReviewerId.ToString());
        var isGuestExist = await guestRepository.AnyAsync(guestId);
        if (!isGuestExist)
            throw new EntityNotFoundException(nameof(Guest), guestId.ToString());
        
        var guestReview = await guestReviewRepository.FindOneAsync(new GuestReviewByIdsSpecification(guestId, createReviewRequest.ReviewerId));
        
        // Update the existing review or create new one
        if(guestReview != null) Mapper.Map(createReviewRequest, guestReview);
        else
        {
            guestReview = Mapper.Map<GuestReview>(createReviewRequest);
            guestReviewRepository.Add(guestReview);
        }

        await unitOfWork.SaveChangesAsync();
        var reviewResponse = Mapper.Map<GetReviewResponse>(guestReview);
        reviewResponse.ReviewerName = currentUser.Name!;
        reviewResponse.ReviewerAvatarUrl = currentUser.AvatarUrl!;
        
        return reviewResponse;
    }

    public async Task DeleteGuestReviewAsync(int guestReviewId)
    {
        var guestReview = await guestReviewRepository.GetByIdAsync(guestReviewId)
            ?? throw new EntityNotFoundException(nameof(GuestReview), guestReviewId.ToString());
        
        // Check user have permission to delete the review
        var currentUserId = int.Parse(currentUser.Id!);
        var currentUserRole = currentUser.Role!;
        Console.WriteLine("currentUserRole: " + currentUserRole);

        if (currentUserRole != AppRole.Admin)
        {
            var hostId = (await hostRepository.FindOneAsync(new HostByUserIdSpecification(currentUserId)))!.Id;
            if(guestReview.HostId != hostId) 
                throw new ForbiddenAccessException("You do not have permission to delete this review");
        }
        guestReviewRepository.Delete(guestReview);
        await unitOfWork.SaveChangesAsync();
    }

}


public class ReviewMapping : Profile
{
    public ReviewMapping()
    {
        CreateMap<PropertyReview, GetPropertyReviewResponse>()
            .ForMember(res => res.GuestName, opt => opt.MapFrom(hr => hr.Guest.User.FullName))
            .ForMember(res => res.UserId, opt => opt.MapFrom(hr => hr.Guest.UserId))
            .ForMember(res => res.GuestAvatarUrl, opt => opt.MapFrom(hr => hr.Guest.User.AvatarUrl))
            .ForMember(res => res.ReviewTime, opt => opt.MapFrom(hr => hr.LastModifiedAt))
            .ForMember(res => res.AverageRating, opt => opt
                .MapFrom(hr => ((double)(hr.Cleanliness + hr.Communication + hr.CheckIn + hr.Accuracy + hr.Location + hr.Value)) / 6));
        
        CreateMap<CreatePropertyReviewRequest, PropertyReview>();
        
        CreateMap<HostReview, GetReviewResponse>()
            .ForMember(res => res.ReviewerId, opt => opt.MapFrom(hr => hr.GuestId))
            .ForMember(res => res.ReviewerName, opt => opt.MapFrom(hr => hr.Guest.User.FullName))
            .ForMember(res => res.UserId, opt => opt.MapFrom(hr => hr.Guest.UserId))
            .ForMember(res => res.ReviewerAvatarUrl, opt => opt.MapFrom(hr => hr.Guest.User.AvatarUrl))
            .ForMember(res => res.ReviewTime, opt => opt.MapFrom(hr => hr.LastModifiedAt));
        
        CreateMap<CreateReviewRequest, HostReview>()
            .ForMember(hr => hr.HostId, opt => opt.MapFrom(req => req.RevieweeId))
            .ForMember(hr => hr.GuestId, opt => opt.MapFrom(req => req.ReviewerId));
        
        CreateMap<GuestReview, GetReviewResponse>()
            .ForMember(res => res.ReviewerId, opt => opt.MapFrom(gr => gr.HostId))
            .ForMember(res => res.UserId, opt => opt.MapFrom(gr => gr.Host.UserId))
            .ForMember(res => res.ReviewerName, opt => opt.MapFrom(gr => gr.Host.User.FullName))
            .ForMember(res => res.ReviewerAvatarUrl, opt => opt.MapFrom(gr => gr.Host.User.AvatarUrl))
            .ForMember(res => res.ReviewTime, opt => opt.MapFrom(gr => gr.LastModifiedAt));
        
        CreateMap<CreateReviewRequest, GuestReview>()
            .ForMember(gr => gr.GuestId, opt => opt.MapFrom(req => req.RevieweeId))
            .ForMember(gr => gr.HostId, opt => opt.MapFrom(req => req.ReviewerId));
    }
}