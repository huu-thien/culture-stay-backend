using CultureStay.Domain.Enum;
using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.Booking.Specifications;

public class IsGuestStayedInHostPropertySpecification : Specification<Domain.Entities.Booking>
{
    public IsGuestStayedInHostPropertySpecification(int hostId, int guestId)
    {
        AddFilter(b => b.GuestId == guestId);
        AddFilter(b => b.Property.HostId == hostId);
        AddFilter(b => b.Status == BookingStatus.Completed || b.Status == BookingStatus.CancelledAfterCheckIn);
    }
}