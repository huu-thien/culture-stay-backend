using AutoMapper;
using CultureStay.Application.Common;
using CultureStay.Application.Common.Interfaces;
using CultureStay.Application.Common.Models;
using CultureStay.Application.Common.Specifications;
using CultureStay.Application.Services.Interface;
using CultureStay.Application.ViewModels.Booking.QueryParameters;
using CultureStay.Application.ViewModels.Booking.Request;
using CultureStay.Application.ViewModels.Booking.Response;
using CultureStay.Application.ViewModels.Booking.Specifications;
using CultureStay.Domain.Entities;
using CultureStay.Domain.Enum;
using CultureStay.Domain.Exceptions;
using CultureStay.Domain.Exceptions.BookingException;
using CultureStay.Domain.Repositories.Base;
using Microsoft.Extensions.Logging;

namespace CultureStay.Application.Services;

public class BookingService (
    IRepositoryBase<Booking> bookingRepository,
    IRepositoryBase<Property> propertyRepository,
    IRepositoryBase<Guest> guestRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<BookingService> logger,
    CultureStaySettings cultureStaySettings,
    ICurrentUser currentUser
    ) : BaseService(unitOfWork, mapper, currentUser), IBookingService
{
    public async Task<PaginatedList<GetBookingForHostResponse>> GetBookingsForHostAsync(int hostId, BookingQueryParameters bqp)
    {
        var spec = new GetBookingsForHostSpecification(hostId, bqp);
        
        var (bookings, totalCount) = await bookingRepository.FindWithTotalCountAsync(spec);
        
        var result = Mapper.Map<List<GetBookingForHostResponse>>(bookings);
        return new PaginatedList<GetBookingForHostResponse>(result, totalCount, bqp.PageIndex, bqp.PageSize);
    }

    public async Task<PaginatedList<GetBookingForGuestResponse>> GetBookingsForGuestAsync(int guestId, BookingQueryParameters bqp)
    {
        var spec = new GetBookingsForGuestSpecification(guestId, bqp);
        var (bookings, totalCount) = await bookingRepository.FindWithTotalCountAsync(spec);
        
        var result = Mapper.Map<List<GetBookingForGuestResponse>>(bookings);
        return new PaginatedList<GetBookingForGuestResponse>(result, totalCount, bqp.PageIndex, bqp.PageSize);
    }

    public async Task<GetDraftBookingResponse> CreateBookingAsync(CreateBookingRequest request)
    {
        logger.LogInformation("Creating booking {@Booking}", request);
        var guest = await guestRepository.FindOneAsync(new GuestByUserIdSpecification(int.Parse(currentUser.Id!)));
        if (guest is null)
            throw new EntityNotFoundException(nameof(Guest), currentUser.Id!);
        request.GuestId = guest.Id;
        
        // Get property info
        var propertyInfo = await propertyRepository.FindOneAsync(new GetPropertyWithHostSpecification(request.PropertyId))
                ?? throw new EntityNotFoundException(nameof(Property), request.PropertyId.ToString());
        
        var userId = int.Parse(currentUser.Id!);
        request.GuestId = (await guestRepository.FindOneAsync(new GuestByUserIdSpecification(userId)))?.Id
                         ?? throw new EntityNotFoundException(nameof(Guest), userId.ToString());
        
        // Validate input
        await ValidateInput(request, propertyInfo);
        
        // Map property info to booking
        var booking = Mapper.Map<Booking>(request);
        booking.PricePerNight = propertyInfo.PricePerNight;
        booking.SystemFee = cultureStaySettings.SystemFee;
        
        booking.Status = BookingStatus.Pending;
        
        // Calculate total price
        var numberOfDays = (booking.CheckOutDate.Date - booking.CheckInDate.Date).Days;
        var stayPrice = booking.PricePerNight * numberOfDays;
        booking.TotalPrice = stayPrice;
        
        // Save booking
        bookingRepository.Add(booking);
        await unitOfWork.SaveChangesAsync();
        
        return Mapper.Map<GetDraftBookingResponse>(booking);
    }

    public async Task<List<GetBookingForPropertyResponse>> GetBookingsForPropertyAsync(int propertyId, DateTime fromDate, DateTime toDate)
    {
        var spec = new ActiveBookingBetweenDatesSpecification(propertyId, fromDate, toDate);
        var bookings = await bookingRepository.FindListAsync(spec);
        
        return Mapper.Map<List<GetBookingForPropertyResponse>>(bookings);
    }

    public async Task ChangeBookingStatusAsync(int bookingId, UpdateStatusBookingRequest request)
    {
        var booking = await bookingRepository.GetByIdAsync(bookingId)
            ?? throw new EntityNotFoundException(nameof(Booking), bookingId.ToString());
        
        booking.Status = request.Status;
        await unitOfWork.SaveChangesAsync();
    }
    
    private async Task ValidateInput(CreateBookingRequest request, Property property)
    {
        // Check if checkin and checkout date is valid
        if(request.CheckInDate.Date >= request.CheckOutDate.Date)
            throw new InvalidBookingDateException("Checkin date must be before checkout date");
        // Check if checkin date is after today
        if(request.CheckInDate.Date <= DateTime.Now.Date)
            throw new InvalidBookingDateException("Checkin date must be in the future");
        
        // Check if property is available for booking
        var isPropertyAlreadyBooked = 
            await bookingRepository.AnyAsync(new ActiveBookingBetweenDatesSpecification(request.PropertyId, request.CheckInDate, request.CheckOutDate));
        if(isPropertyAlreadyBooked)
            throw new InvalidBookingDateException("Property is already booked for the selected dates");
        
        // Check if number of guests is valid
        if(request.NumberOfGuest > property.MaxGuestCount)
            throw new InvalidBookingDateException($"Number of guest must be less than {property.MaxGuestCount}");
        
        // Check if guest is booking his own property
        var guestUserId = int.Parse(currentUser.Id!);
        if(guestUserId == property.Host.UserId)
            throw new GuestIsHostException("Guest cannot book his own property");
    }
}