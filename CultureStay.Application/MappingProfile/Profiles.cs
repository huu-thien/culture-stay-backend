﻿using AutoMapper;
using CultureStay.Application.ViewModels.Auth.Responses;
using CultureStay.Application.ViewModels.Booking.Request;
using CultureStay.Application.ViewModels.Booking.Response;
using CultureStay.Application.ViewModels.Cancellation.Request;
using CultureStay.Application.ViewModels.Cancellation.Response;
using CultureStay.Application.ViewModels.Chat.Response;
using CultureStay.Application.ViewModels.Guest.Response;
using CultureStay.Application.ViewModels.Host.Response;
using CultureStay.Application.ViewModels.HostPayment.Response;
using CultureStay.Application.ViewModels.Payment.Response;
using CultureStay.Application.ViewModels.PaymentInfo.Response;
using CultureStay.Application.ViewModels.Property.Request;
using CultureStay.Application.ViewModels.User.Request;
using CultureStay.Application.ViewModels.User.Response;
using CultureStay.Domain.Entities;
using CultureStay.Domain.Enum;

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
        
        CreateMap<Guest, GetGuestResponse>()
            .ForMember(res => res.Name, opt => opt.MapFrom(g => g.User.FullName))
            .ForMember(res => res.Introduction, opt => opt.MapFrom(g => g.User.Introduction))
            .ForMember(res => res.AvatarUrl, opt => opt.MapFrom(g => g.User.AvatarUrl))
            .ForMember(res => res.Address, opt => opt.MapFrom(g => g.User.Address))
            .ForMember(res => res.City, opt => opt.MapFrom(g => g.User.City))
            .ForMember(res=> res.JoinedAt, opt => opt.MapFrom(g => g.CreatedAt));
        
        
        CreateMap<Host, GetHostForAdminResponse>()
            .ForMember(res => res.Name, opt => opt.MapFrom(h => h.User.FullName))
            .ForMember(res => res.Address, opt => opt.MapFrom(h => h.User.Address))
            .ForMember(res => res.City, opt => opt.MapFrom(h => h.User.City))
            .ForMember(res=> res.JoinedAt, opt => opt.MapFrom(h => h.CreatedAt));

        CreateMap<User, GetUsersForAdminResponse>()
            .ForMember(res => res.CreatedAt, opt => opt.MapFrom(u => u.Guest.CreatedAt));
        CreateMap<UpdateUserInfoRequest, User>();

        CreateMap<Booking, GetBookingForHostResponse>()
            .ForMember(res => res.GuestName, opt => opt.MapFrom(b => b.Guest.User.FullName))
            .ForMember(res => res.GuestEmail, opt => opt.MapFrom(b => b.Guest.User.Email))
            .ForMember(res => res.GuestPhoneNumber, opt => opt.MapFrom(b => b.Guest.User.PhoneNumber))
            .ForMember(res => res.PropertyName, opt => opt.MapFrom(b => b.Property.Title))
            .ForMember(res => res.NumberOfDays, opt => opt.MapFrom(b => b.CheckOutDate.Subtract(b.CheckInDate).Days))
            .ForMember(res => res.NumberOfGuest, opt => opt.MapFrom(b => b.NumberOfGuests));
        
        CreateMap<Booking, GetBookingForGuestResponse>()
            .ForMember(res => res.HostId, opt => opt.MapFrom(b => b.Property.HostId))
            .ForMember(res => res.HostName, opt => opt.MapFrom(b => b.Property.Host.User.FullName))
            .ForMember(res=> res.HostEmail, opt => opt.MapFrom(b => b.Property.Host.User.Email))
            .ForMember(res => res.HostPhoneNumber, opt => opt.MapFrom(b => b.Property.Host.User.PhoneNumber))
            .ForMember(res => res.PropertyName, opt => opt.MapFrom(b => b.Property.Title))
            .ForMember(res => res.NumberOfDays, opt => opt.MapFrom(b => b.CheckOutDate.Subtract(b.CheckInDate).Days))
            .ForMember(res => res.CheckInCode, opt => opt.MapFrom(b => b.Guid))
            .ForMember(res => res.NumberOfGuest, opt => opt.MapFrom(b => b.NumberOfGuests));
        
        CreateMap<Booking, GetDraftBookingResponse>()
            .ForMember(res => res.PropertyName, opt => opt.MapFrom(b => b.Property.Title))
            .ForMember(res => res.NumberOfDays, opt => opt.MapFrom(b => b.CheckOutDate.Subtract(b.CheckInDate).Days))
            .ForMember(res => res.NumberOfGuest, opt => opt.MapFrom(b => b.NumberOfGuests));

        CreateMap<Booking, GetBookingForPropertyResponse>();
        CreateMap<CreateBookingRequest, Booking>();
        
        CreateMap<CreateCancellationRequest, CancellationTicket>()
            .ForMember(ct => ct.CancellationReasonNote, opt => opt.MapFrom(c => c.Reason));

        CreateMap<CancellationTicket, GetCancellationResponse>()
            .ForMember(res => res.IssuerId, opt => opt.MapFrom(ct => ct.CreatedBy));
           
        
        
        CreateMap<Message, GetMessageResponse>()
            .ForMember(res => res.MessageTime, opt => opt.MapFrom(m => m.CreatedAt));

        CreateMap<User, GetContactResponse>();
        
        
        CreateMap<VnpHistoryDto, VnpHistory>();
        CreateMap<VnpHistory, VnpHistoryDto>();
        
        CreateMap<HostPayment, HostPaymentResponse>();
        CreateMap<PaymentInfo, HostPaymentInfoResponse>();
        CreateMap<PaymentInfoRequest, PaymentInfo>();
        
        CreateMap<PaymentInfo, PaymentInfoResponse>().ReverseMap();
        
        CreateMap<ChargePayment, GetChargePaymentResponse>()
            .ForMember(res=>res.FullName,opt=>opt.MapFrom(src=>src.Host.User.FullName))
            .ForMember(res=>res.UserId,opt=>opt.MapFrom(src=>src.Host.UserId));
        CreateMap<RefundPayment,GetChargePaymentResponse>()
            .ForMember(res => res.FullName, opt => opt.MapFrom(src => src.Guest.User.FullName))
            .ForMember(res=>res.UserId,opt=>opt.MapFrom(src=>src.Guest.UserId));
    }
}