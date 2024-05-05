using CultureStay.Domain.Entities.Base;

namespace CultureStay.Domain.Entities;

public class Guest : EntityBase
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public bool IsBlocked { get; set; }
    
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<GuestReview> GuestReviews { get; set; } = new List<GuestReview>();
    public ICollection<HostReview> HostReviews { get; set; } = new List<HostReview>();
    public ICollection<PropertyReview> PropertyReviews { get; set; } = new List<PropertyReview>();
    public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}