using CultureStay.Application.Common.Models;
using CultureStay.Application.ViewModels.Property.Request;
using CultureStay.Application.ViewModels.Property.Response;
using CultureStay.Application.ViewModels.PropertyUtility.Response;

namespace CultureStay.Application.Services.Interface;

public interface IPropertyService
{
    Task<PaginatedList<GetPropertyResponse>> GetListPropertyAsync(PropertyQueryParameters propertyQueryParameters);
    Task<GetPropertyResponse> GetPropertyByIdAsync(int id);
    Task<PaginatedList<GetPropertyResponse>> GetListPropertyByHostIdAsync(int hostId, PropertyQueryParameters propertyQueryParameters);
    
    Task<bool> IsStayedAsync(int propertyId);
    
    Task<GetPropertyResponse> CreatePropertyAsync(CreatePropertyRequest createPropertyRequest);
    
    Task<GetPropertyResponse> UpdatePropertyAsync(int id, CreatePropertyRequest updatePropertyRequest);
    
    Task DeletePropertyAsync(int id);
}