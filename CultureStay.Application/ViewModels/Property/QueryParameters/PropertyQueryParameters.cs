using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using CultureStay.Application.Common;
using CultureStay.Application.ViewModels.Property.Enums;
using CultureStay.Domain.Enum;

namespace CultureStay.Application.ViewModels.PropertyUtility.Response;

public class PropertyQueryParameters : PagingParameters
{
    // Search theo loai phong
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public List<PropertyType>? Type { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PropertySortBy? OrderBy { get; set; }
    
    // Search theo so luong phong, so giuong, so phong tam
    [Range(0, 10)]
    public int MinBedroomCount { get; set; } = 0;
    [Range(0, 10)]
    public int MaxBedroomCount { get; set; } = 0;
    [Range(0, 10)]
    public int MinBedCount { get; set; } = 0;
    [Range(0, 10)]
    public int MaxBedCount { get; set; } = 0;
    [Range(0, 10)]
    public int MinBathroomCount { get; set; } = 0;
    [Range(0, 10)]
    public int MaxBathroomCount { get; set; } = 0;
    
    // Search theo khu vuc
    public string? City { get; set; }
    
    // Search theo khoang ngay
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    
    // Search theo so luong nguoi
    [Range(0, 64)]
    public int GuestCount { get; set; } = 0;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PropertyStatus? Status { get; set; }
}