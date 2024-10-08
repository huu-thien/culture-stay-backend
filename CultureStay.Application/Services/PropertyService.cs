﻿using AutoMapper;
using CultureStay.Application.Common.Interfaces;
using CultureStay.Application.Common.Models;
using CultureStay.Application.Common.Specifications;
using CultureStay.Application.Services.Interface;
using CultureStay.Application.ViewModels.Booking.Specifications;
using CultureStay.Application.ViewModels.Property.Request;
using CultureStay.Application.ViewModels.Property.Response;
using CultureStay.Application.ViewModels.Property.Specifications;
using CultureStay.Application.ViewModels.PropertyImage.Response;
using CultureStay.Application.ViewModels.PropertyUtility.Response;
using CultureStay.Domain.Entities;
using CultureStay.Domain.Enum;
using CultureStay.Domain.Exceptions;
using CultureStay.Domain.Repositories.Base;

namespace CultureStay.Application.Services;

public class PropertyService (
    IRepositoryBase<Property> propertyRepository,
    IRepositoryBase<Host> hostRepository,
    IRepositoryBase<Guest> guestRepository,
    IRepositoryBase<Booking> bookingRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentUser currentUser) : BaseService(unitOfWork, mapper, currentUser), IPropertyService
{

    
    public async Task<PaginatedList<GetPropertyResponse>> GetListPropertyAsync(PropertyQueryParameters pqp)
    {
        var role = currentUser.Role;
        Console.WriteLine("rol ne: " + role);
        var propertyFilterSpec = new PropertyFilterSpecification(pqp, role);
        
        var (items, totalCount) = await propertyRepository.FindWithTotalCountAsync(propertyFilterSpec);

        var propertyList = items.ToList();

        var result = Mapper.Map<List<GetPropertyResponse>>(propertyList);
        
        // If logged in user, show favorite properties
        var currentGuestId = 0;
        if (!string.IsNullOrWhiteSpace(currentUser.Id))
        {
            var currentGuest = await guestRepository.FindOneAsync(new GuestByUserIdSpecification(int.Parse(currentUser.Id)));
            if (currentGuest is not null)
                currentGuestId = currentGuest.Id;
            else
                currentGuestId = 0;
        }

        foreach (var item in result)
        {
            item.NumberOfReviews = propertyList.First(i => i.Id == item.Id).PropertyReviews.Count;
            if(item.NumberOfReviews == 0) continue;
            var property = propertyList.First(i => i.Id == item.Id);
            item.Rating = property.PropertyReviews
                .Average(r => (r.Accuracy + r.Communication + r.Cleanliness + r.Location + r.CheckIn + r.Value) / 6.0);
            item.IsFavorite = propertyList.Any(i => i.Id == item.Id && i.Wishlists.Any(w => w.GuestId == currentGuestId));
        }
        
        return new PaginatedList<GetPropertyResponse>(result, totalCount, pqp.PageIndex, pqp.PageSize);
    }

    public async Task<GetPropertyResponse> GetPropertyByIdAsync(int id)
    {
        var propertyFilterSpec = new PropertyDetailSpecification(id);
        var property = await propertyRepository.FindOneAsync(propertyFilterSpec)
                        ?? throw new EntityNotFoundException(nameof(Property), id.ToString());

        var result = Mapper.Map<GetPropertyResponse>(property);
        result.NumberOfReviews = property.PropertyReviews.Count;
        if (result.NumberOfReviews == 0) return result;
        result.Rating = property.PropertyReviews
            .Average(r => (r.Accuracy + r.Communication + r.Cleanliness + r.Location + r.CheckIn + r.Value) / 6.0);
        
        // If logged in user, show isFavorite property
        if (string.IsNullOrWhiteSpace(currentUser.Id)) return result;
        var currentGuestId = (await guestRepository.FindOneAsync(new GuestByUserIdSpecification(int.Parse(currentUser.Id))))!.Id;
        result.IsFavorite = property.Wishlists.Any(w => w.GuestId == currentGuestId);
        return result;
    }

    public async Task<PaginatedList<GetPropertyResponse>> GetListPropertyByHostIdAsync(int hostId, PropertyQueryParameters pqp)
    {
        var role = currentUser.Role;
        var propertyFilterSpec = new PropertyFilterSpecification(pqp, role, hostId);
        
        var (items, totalCount) = await propertyRepository.FindWithTotalCountAsync(propertyFilterSpec);
        var propertyList = items.ToList();

        var result = Mapper.Map<List<GetPropertyResponse>>(propertyList);
        
        // If logged in user, show favorite properties
        var currentGuestId = 0;
        if (!string.IsNullOrWhiteSpace(currentUser.Id))
        {
            currentGuestId =
                (await guestRepository.FindOneAsync(new GuestByUserIdSpecification(int.Parse(currentUser.Id))))!.Id;
        }

        foreach (var item in result)
        {
            item.NumberOfReviews = propertyList.First(i => i.Id == item.Id).PropertyReviews.Count;
            if(item.NumberOfReviews == 0) continue;
            var property = propertyList.First(i => i.Id == item.Id);
            item.Rating = property.PropertyReviews
                .Average(r => (r.Accuracy + r.Communication + r.Cleanliness + r.Location + r.CheckIn + r.Value) / 6.0);
            item.IsFavorite = propertyList.Any(i => i.Id == item.Id && i.Wishlists.Any(w => w.GuestId == currentGuestId));
        }
        
        return new PaginatedList<GetPropertyResponse>(result, totalCount, pqp.PageIndex, pqp.PageSize);
    }

    public async Task<bool> IsStayedAsync(int propertyId)
    {
        var currentUserId = int.TryParse(currentUser.Id, out var id) ? id : 0;
        if(currentUserId == 0) return false;
        
        var guest = await guestRepository.FindOneAsync(new GuestByUserIdSpecification(currentUserId));
        if(guest is null) return false;
        
        var spec = new IsGuestStayedSpecification(propertyId, guest.Id);
        return await bookingRepository.AnyAsync(spec);
    }

    public async Task<GetPropertyResponse> CreatePropertyAsync(CreatePropertyRequest request)
    {
        // Get hostId from current user
        var userId = int.Parse(currentUser.Id!);
        var host = await hostRepository.FindOneAsync(new HostByUserIdSpecification(userId));
        
        // Create host profile if not exist
        if (host is null)
        {
            var paymentInfo = Mapper.Map<PaymentInfo>(request.PaymentInfo);
            host = new Host {UserId = userId,PaymentInfo = paymentInfo};
            hostRepository.Add(host);
        }
        
        // Create property status pending
        var property = Mapper.Map<Property>(request);
        var propertyUtilities = Mapper.Map<PropertyUtility>(request.PropertyUtilities);
        property.PropertyUtilities.Add(propertyUtilities);
        property.Status = PropertyStatus.Approved;
        host.Properties.Add(property);
        
        await UnitOfWork.SaveChangesAsync();
        return Mapper.Map<GetPropertyResponse>(property);
    }

    public async Task<GetPropertyResponse> UpdatePropertyAsync(int id, CreatePropertyRequest request)
    {
        var property = await propertyRepository.GetByIdAsync(id)
                       ?? throw new EntityNotFoundException(nameof(Property), id.ToString());
        
        Mapper.Map(request, property);
        await unitOfWork.SaveChangesAsync();
        return Mapper.Map<GetPropertyResponse>(property);
    }

    public async Task DeletePropertyAsync(int id)
    {
        var property = await propertyRepository.GetByIdAsync(id)
                       ?? throw new EntityNotFoundException(nameof(Property), id.ToString()); 
        propertyRepository.Delete(property);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task ConfirmCreatePropertyRequestAsync(int propertyId)
    {
        var property = await propertyRepository.GetByIdAsync(propertyId)
                       ?? throw new EntityNotFoundException(nameof(Property), propertyId.ToString());
        
        property.Status = PropertyStatus.Approved;
        await unitOfWork.SaveChangesAsync();
    }

    public async Task RejectCreatePropertyRequestAsync(int propertyId, RejectPropertyRequest request)
    {
        var property = await propertyRepository.GetByIdAsync(propertyId)
                       ?? throw new EntityNotFoundException(nameof(Property), propertyId.ToString());
        
        property.Status = PropertyStatus.Rejected;
        property.RejectionReason = request.Reason;
        await unitOfWork.SaveChangesAsync();
    }
}


public class PropertyMapping : Profile
{
    public PropertyMapping()
    {
        CreateMap<Property, GetPropertyResponse>()
            .ForMember(res => res.HostName, opt => opt.MapFrom(p => p.Host.User.FullName));

        CreateMap<PropertyImage, GetPropertyImageResponse>();
        CreateMap<PropertyUtility, GetPropertyUtilityResponse>();
        CreateMap<CreatePropertyRequest, Property>()
            .ForMember(p => p.PropertyUtilities, opt => opt.Ignore());
        CreateMap<CreatePropertyImageRequest, PropertyImage>();
        CreateMap<PropertyUtilityResponse, PropertyUtility>();
    }
}
