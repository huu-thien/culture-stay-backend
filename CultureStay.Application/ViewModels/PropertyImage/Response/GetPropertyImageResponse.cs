using AutoMapper;
using CultureStay.Application.ViewModels.Property.Response;

namespace CultureStay.Application.ViewModels.PropertyImage.Response;

public class GetPropertyImageResponse
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public int PropertyId { get; set; }
}

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Domain.Entities.PropertyImage, GetPropertyImageResponse>();
    }
}