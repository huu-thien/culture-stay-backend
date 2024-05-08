using CultureStay.Application.Common.Models;
using CultureStay.Application.ViewModels.Property.Response;
using CultureStay.Application.ViewModels.PropertyUtility.Response;

namespace CultureStay.Application.Services.Interface;

public interface IPropertyService
{
    Task<PaginatedList<GetListPropertyResponse>> GetListPropertyAsync(PropertyQueryParameters propertyQueryParameters);
}