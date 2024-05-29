using System.Text.Json.Serialization;
using CultureStay.Application.Common;
using CultureStay.Application.ViewModels.Booking.Enums;
using CultureStay.Domain.Enum;

namespace CultureStay.Application.ViewModels.Booking.QueryParameters;

public class BookingQueryParameters : PagingParameters
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BookingSortBy? OrderBy { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BookingStatus? Status { get; set; }
}