using AutoMapper;
using CultureStay.Application.Common;
using CultureStay.Application.Common.Interfaces;
using CultureStay.Application.Common.Models;
using CultureStay.Application.Common.Specifications;
using CultureStay.Application.Services.Interface;
using CultureStay.Application.ViewModels.Property.Response;
using CultureStay.Application.ViewModels.Property.Specifications;
using CultureStay.Application.ViewModels.Wishlist.Specifications;
using CultureStay.Domain.Entities;
using CultureStay.Domain.Exceptions;
using CultureStay.Domain.Repositories.Base;

namespace CultureStay.Application.Services;

public class WishlistService(
        IRepositoryBase<Wishlist> wishlistRepository,
        IRepositoryBase<Guest> guestRepository,
        IRepositoryBase<Property> propertyRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUser currentUser
    ) : BaseService(unitOfWork, mapper, currentUser) , IWishlistService
{

    private async Task<Guest> GetGuest(int userId)
    {
        var speccification = new GuestByUserIdSpecification(userId);
        var guest = await guestRepository.FindOneAsync(speccification);
        if(guest is null)
            throw new EntityNotFoundException(nameof(Guest), userId.ToString());
        return guest;
    }
    
    public async Task<PaginatedList<GetPropertyResponse>> GetListWishlistAsync(PagingParameters pg)
    {
        var guest = await GetGuest(int.Parse(currentUser.Id!));
        var specification = new WishlistByGuestIdSpecification(pg, guest.Id);
        var items = await wishlistRepository.FindListAsync(specification);
        
        var propertySpecification = new PropertiesByIdsSpecification(items.Select(w => w.PropertyId).ToList(), pg.PageIndex, pg.PageSize);
        var (properties, totalCount) = await propertyRepository.FindWithTotalCountAsync(propertySpecification);
        
        var propertyList = properties.ToList();
        var result = Mapper.Map<List<GetPropertyResponse>>(propertyList);

        foreach (var item in result)
        {
            item.NumberOfReviews = propertyList.First(i => i.Id == item.Id).PropertyReviews.Count;
            if(item.NumberOfReviews == 0) continue;
            var property = propertyList.First(i => i.Id == item.Id);
            item.Rating = property.PropertyReviews
                .Average(r => (r.Accuracy + r.Communication + r.Cleanliness + r.Location + r.CheckIn + r.Value) / 6.0);
        }
        result.ForEach(item => item.IsFavorite = true);
        return new PaginatedList<GetPropertyResponse>(result, totalCount, pg.PageIndex, pg.PageSize);
    }

    public async Task AddWishLlistAsync(int propertyId)
    {
        var guest = await GetGuest(int.Parse(currentUser.Id!));
        var property = await propertyRepository.GetByIdAsync(propertyId);
        if(property is null)
            throw new EntityNotFoundException(nameof(Property), propertyId.ToString());
        
        var specification = new WishlistByGuestIdAndPropertyIdSpecification(guest.Id, propertyId, true);
        var wishlist = await wishlistRepository.FindOneAsync(specification);

        if (wishlist is null)
        {
            var newWishList = new Wishlist
            {
                GuestId = guest.Id,
                PropertyId = propertyId
            };
            wishlistRepository.Add(newWishList);
            await unitOfWork.SaveChangesAsync();
            return;
        }
        wishlist.IsDeleted = false;
        await unitOfWork.SaveChangesAsync();
    }

    public async Task RemoveWishlistAsync(int propertyId)
    {
        var guest = await GetGuest(int.Parse(currentUser.Id!));
        var specification = new WishlistByGuestIdAndPropertyIdSpecification(guest.Id, propertyId);
        var wishlist = await wishlistRepository.FindOneAsync(specification)
                        ?? throw new EntityNotFoundException("Wishlist item", $"property: {propertyId} - userId: {currentUser.Id}");
        wishlistRepository.Delete(wishlist);
        await unitOfWork.SaveChangesAsync();
    }
}