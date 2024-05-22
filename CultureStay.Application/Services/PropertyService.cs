using AutoMapper;
using CultureStay.Application.Common.Interfaces;
using CultureStay.Application.Common.Models;
using CultureStay.Application.Common.Specifications;
using CultureStay.Application.Services.Interface;
using CultureStay.Application.ViewModels.Booking.Specifications;
using CultureStay.Application.ViewModels.Property.Response;
using CultureStay.Application.ViewModels.Property.Specifications;
using CultureStay.Application.ViewModels.PropertyUtility.Response;
using CultureStay.Domain.Entities;
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
}


public class PropertyMapping : Profile
{
    public PropertyMapping()
    {
        CreateMap<Domain.Entities.Property, GetPropertyResponse>()
            .ForMember(res => res.HostName, opt => opt.MapFrom(p => p.Host.User.FullName));
    }
}
