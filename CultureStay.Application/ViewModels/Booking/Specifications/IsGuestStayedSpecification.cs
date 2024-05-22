using CultureStay.Domain.Enum;
using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.Booking.Specifications;

public class IsGuestStayedSpecification : Specification<Domain.Entities.Booking>
{
    public IsGuestStayedSpecification(int propertyId, int guestId)
    {
        AddFilter(b => b.GuestId == guestId);
        AddFilter(b => b.PropertyId == propertyId);
        AddFilter(b => b.Status == BookingStatus.Completed || b.Status == BookingStatus.CancelledAfterCheckIn);
    }
}