using AutoMapper;
using CultureStay.Application.ViewModels.PropertyImage.Response;
using CultureStay.Application.ViewModels.PropertyUtility.Response;
using CultureStay.Domain.Enum;
using Newtonsoft.Json;

namespace CultureStay.Application.ViewModels.Property.Response;

public class GetListPropertyResponse
{
    public int Id { get; set; }
    public PropertyType Type { get; set; }
    
    public int BedCount { get; set; }
    public int BathroomCount { get; set; }
    public int MaxGuestCount { get; set; }
    
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    
    public bool IsFavorite { get; set; }
    
    public int HostId { get; set; }
    public string HostName { get; set; } = string.Empty;
    
    public double Rating { get; set; }
    public int NumberOfReviews { get; set; }
    
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    public ICollection<GetPropertyImageResponse>? PropertyImages { get; set; }
    
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public ICollection<GetPropertyUtilityResponse>? PropertyUtilities { get; set; }
    
    public PropertyStatus Status { get; set; }
    public string? RejectionReason { get; set; }
}

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Domain.Entities.Property, GetListPropertyResponse>();
    }
}