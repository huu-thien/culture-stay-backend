using System.ComponentModel.DataAnnotations;

namespace CultureStay.Application.ViewModels.Property.Request;

public class RejectPropertyRequest
{
    [Required]
    public string? Reason { get; set; }
}