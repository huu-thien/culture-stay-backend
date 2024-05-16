using AutoMapper;
using CultureStay.Application.ViewModels.Auth.Responses;
using CultureStay.Application.ViewModels.Host.Response;
using CultureStay.Domain.Entities;

namespace CultureStay.Application.MappingProfile;

public class Profiles : Profile
{
    public Profiles()
    {
        CreateMap<User, GetUserResponse>();
        
        CreateMap<Host, GetHostResponse>()
            .ForMember(res => res.Name, opt => opt.MapFrom(h => h.User.FullName))
            .ForMember(res => res.Introduction, opt => opt.MapFrom(h => h.User.Introduction))
            .ForMember(res => res.AvatarUrl, opt => opt.MapFrom(h => h.User.AvatarUrl))
            .ForMember(res => res.Address, opt => opt.MapFrom(h => h.User.Address))
            .ForMember(res => res.City, opt => opt.MapFrom(h => h.User.City))
            .ForMember(res=> res.JoinedAt, opt => opt.MapFrom(h => h.CreatedAt));
        
        CreateMap<Host, GetHostForAdminResponse>()
            .ForMember(res => res.Name, opt => opt.MapFrom(h => h.User.FullName))
            .ForMember(res => res.Address, opt => opt.MapFrom(h => h.User.Address))
            .ForMember(res => res.City, opt => opt.MapFrom(h => h.User.City))
            .ForMember(res=> res.JoinedAt, opt => opt.MapFrom(h => h.CreatedAt));
    }
}