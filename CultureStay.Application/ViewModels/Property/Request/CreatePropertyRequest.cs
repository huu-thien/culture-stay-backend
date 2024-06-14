using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using CultureStay.Application.ViewModels.Property.Response;
using CultureStay.Domain.Enum;

namespace CultureStay.Application.ViewModels.Property.Request;

public class CreatePropertyRequest
{
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [EnumDataType(typeof(PropertyType))]
    public PropertyType Type { get; set; }
    
    [Required]
    [Range(1, 10)]
    public int BedCount { get; set; }
    
    [Required]
    [Range(1, 10)]
    public int BedroomCount { get; set; }
    
    [Required]
    [Range(1, 10)]
    public int BathroomCount { get; set; }
    
    [Required]
    [Range(1, 10)]
    public int MaxGuestCount { get; set; }
    
    [Required]
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [Required]
    public double Latitude { get; set; }
    
    [Required]
    public double Longitude { get; set; }
    
    [Required]
    public string Address { get; set; } = string.Empty;
    
    [Required]
    public string City { get; set; } = string.Empty;
    
    
    [Required]
    public ICollection<CreatePropertyImageRequest> PropertyImages { get; set; } = new List<CreatePropertyImageRequest>();
    
    [Required]
    public PropertyUtilityResponse PropertyUtilities { get; set; } = new PropertyUtilityResponse();
    
    [Required]
    public PaymentInfoRequest PaymentInfo { get; set; } = new PaymentInfoRequest();
}