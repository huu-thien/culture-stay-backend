using CultureStay.Application.Common;
using CultureStay.Application.Common.Models;
using CultureStay.Application.ViewModels.Property.Response;

namespace CultureStay.Application.Services.Interface;

public interface IWishlistService
{
    Task<PaginatedList<GetPropertyResponse>> GetListWishlistAsync(PagingParameters pg);
    Task AddWishLlistAsync(int propertyId);
    Task RemoveWishlistAsync(int propertyId);
}