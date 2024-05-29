using CultureStay.Application.Common.Models;
using CultureStay.Application.ViewModels.Booking.QueryParameters;
using CultureStay.Application.ViewModels.Booking.Request;
using CultureStay.Application.ViewModels.Booking.Response;
using CultureStay.Domain.Enum;

namespace CultureStay.Application.Services.Interface;

public interface IBookingService
{
    Task<PaginatedList<GetBookingForHostResponse>> GetBookingsForHostAsync(int hostId, BookingQueryParameters bqp);
    Task<PaginatedList<GetBookingForGuestResponse>> GetBookingsForGuestAsync(int guestId, BookingQueryParameters bqp);
    Task<GetDraftBookingResponse> CreateBookingAsync(CreateBookingRequest createBookingDto);
    Task<List<GetBookingForPropertyResponse>> GetBookingsForPropertyAsync(int propertyId, DateTime fromDate, DateTime toDate);
    Task ChangeBookingStatusAsync(int bookingId, UpdateStatusBookingRequest status);
}