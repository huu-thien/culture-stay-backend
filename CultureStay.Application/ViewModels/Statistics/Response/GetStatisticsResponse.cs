using CultureStay.Domain.Enum;

namespace CultureStay.Application.ViewModels.Statistics.Response;

public class GetStatisticsResponse
{
    public int BookingsCount { get; set; }
    
    public int NewPropertyRequestsCount { get; set; }
    public int NewPropertiesCount { get; set; }
    
    public int CancelledBookingsCount { get; set; }
    public int CancelledBookingBeforeCheckInCount { get; set; }
    public int CancelledBookingAfterCheckInCount { get; set; }
    
    public double TotalRevenue { get; set; }
    public double TotalProfit { get; set; }

    public List<GetPropertyStatDto> Top10Properties { get; set; } = new();
    public List<GetPropertyTypeStatDto> PropertyTypeStats { get; set; } = new();
}

public class GetPropertyTypeStatDto
{
    public PropertyType Type { get; set; }
    public int TotalBookings { get; set; }
    public double TotalRevenue { get; set; }
}

public class GetPropertyStatDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public int BookingsCount { get; set; }
}