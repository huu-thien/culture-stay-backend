using CultureStay.Application.Common.Models;
using CultureStay.Application.Services.Interface;
using CultureStay.Application.ViewModels;
using CultureStay.Application.ViewModels.Booking.QueryParameters;
using CultureStay.Application.ViewModels.Booking.Request;
using CultureStay.Application.ViewModels.Booking.Response;
using CultureStay.Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CultureStay.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    
    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }
    
    [HttpGet("host/{hostId:int}")]
    [ProducesResponseType(typeof(GetBookingForHostResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetListBookingForHostAsync(int hostId, [FromQuery] BookingQueryParameters bqp)
    {
        var result = await _bookingService.GetBookingsForHostAsync(hostId, bqp);
        return Ok(new BaseResponse<PaginatedList<GetBookingForHostResponse>>{Message = "Get booking for guest successfully", Data = result});
    }
    
    [HttpGet("guest/{guestId:int}")]
    [ProducesResponseType(typeof(GetBookingForGuestResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetListBookingForGuestAsync(int guestId, [FromQuery] BookingQueryParameters bqp)
    {
        var result = await _bookingService.GetBookingsForGuestAsync(guestId, bqp);
        return Ok(new BaseResponse<PaginatedList<GetBookingForGuestResponse>>{Message = "Get booking for guest successfully", Data = result});
    }
    
    [HttpGet("property/{propertyId:int}")]
    [ProducesResponseType(typeof(GetBookingForPropertyResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetListBookingForPropertyAsync(int propertyId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
    {
        var result = await _bookingService.GetBookingsForPropertyAsync(propertyId, fromDate, toDate);
        return Ok(new BaseResponse<List<GetBookingForPropertyResponse>>{Message = "Get booking for property successfully", Data = result});
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateBookingAsync([FromBody] CreateBookingRequest request)
    {
        var result = await _bookingService.CreateBookingAsync(request);
        return Ok(new BaseResponse<GetDraftBookingResponse>{Message = "Create booking successfully", Data = result});
    }
    
    [HttpPut("{bookingId:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ChangeBookingStatusAsync(int bookingId, [FromBody] UpdateStatusBookingRequest status)
    {
        await _bookingService.ChangeBookingStatusAsync(bookingId, status);
        return Ok(new BaseResponse{Message = "Change booking status successfully"});
    }
}