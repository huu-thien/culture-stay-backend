using CultureStay.Domain.Entities.Base;

namespace CultureStay.Domain.Entities;

public class GuestReview: EntityBase
{
    public int GuestId { get; set; }
    public Guest Guest { get; set; } = null!;
    public int HostId { get; set; }
    public Host Host { get; set; } = null!;
    public double Rating { get; set; }
    public string? Content { get; set; }
}