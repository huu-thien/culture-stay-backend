using CultureStay.Application.ViewModels;
using CultureStay.Application.ViewModels.Statistics.Request;
using CultureStay.Application.ViewModels.Statistics.Response;
using CultureStay.Domain.Entities;
using CultureStay.Domain.Enum;
using CultureStay.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CultureStay.Controllers;

[ApiController]
[Route("api/statistics")]
public class StatisticsController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public StatisticsController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetStatisticsRequest request)
    {
        var result = new GetStatisticsResponse();
        
        var bookings = await _context.Set<Booking>()
            .Where(x => !request.From.HasValue || x.CreatedAt >= request.From)
            .Where(x => !request.To.HasValue || x.CreatedAt <= request.To)
            .Include(x => x.Property)
            .ToListAsync();
        
        result.BookingsCount = bookings.Count;
        result.CancelledBookingBeforeCheckInCount = bookings.Count(x => x.Status == BookingStatus.CancelledBeforeCheckIn);
        result.CancelledBookingAfterCheckInCount = bookings.Count(x => x.Status == BookingStatus.CancelledAfterCheckIn);
        result.CancelledBookingsCount = result.CancelledBookingBeforeCheckInCount + result.CancelledBookingAfterCheckInCount;
        
        result.TotalRevenue = bookings
            .Where(x => x.Status == BookingStatus.Completed)
            .Sum(x => x.TotalPrice);
        result.TotalProfit = bookings
            .Where(x => x.Status == BookingStatus.Completed)
            .Sum(x => x.TotalPrice) * 0.05;

        result.PropertyTypeStats = bookings.GroupBy(x => x.Property.Type)
            .Select(g => new GetPropertyTypeStatDto
            {
                Type = g.Key,
                TotalBookings = g.Count(),
                TotalRevenue = g.Sum(x => x.TotalPrice)
            })
            .ToList();

        result.NewPropertiesCount = await _context.Set<Property>()
                                        .Where(x => !request.From.HasValue || x.CreatedAt >= request.From)
                                        .Where(x => !request.To.HasValue || x.CreatedAt <= request.To)
                                        .Where(x => x.Status == PropertyStatus.Approved)
                                        .CountAsync();
        
        result.NewPropertyRequestsCount = await _context.Set<Property>()
                                        .Where(x => !request.From.HasValue || x.CreatedAt >= request.From)
                                        .Where(x => !request.To.HasValue || x.CreatedAt <= request.To)
                                        .Where(x => x.Status == PropertyStatus.Approved || x.Status == PropertyStatus.Pending)
                                        .CountAsync();

        result.Top10Properties = await _context.Set<Booking>()
                        .Where(x => !request.From.HasValue || x.CreatedAt >= request.From)
                        .Where(x => !request.To.HasValue || x.CreatedAt <= request.To)
                        .GroupBy(x => x.Property)
                        .OrderByDescending(g => g.Count())
                        .Take(10)
                        .Select(g => new GetPropertyStatDto
                        {
                            Id = g.Key.Id,
                            Title = g.Key.Title,
                            BookingsCount = g.Count()
                        })
                        .ToListAsync();

        return Ok(new BaseResponse<GetStatisticsResponse>{Message = "Success", Data = result});
    }
}