using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using CultureStay.Domain;

namespace CultureStay.Application.ViewModels.Booking.Request;

public class CreateBookingRequest
{
    public int PropertyId { get; set; }
    
    [JsonIgnore]
    public int GuestId { get; set; }
    
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    
    [Range(1, 10)]
    public int NumberOfGuest{ get; set; }
    
    [MaxLength(StringLength.Description)]
    public string? Note { get; set; }
}