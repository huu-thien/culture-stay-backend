using CultureStay.Domain.Enum;
using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.Booking.Specifications;

public class ExpiredBookingSpecification : Specification<Domain.Entities.Booking>
{
    public ExpiredBookingSpecification()
    {
        AddFilter(p => p.Status == BookingStatus.Pending);
        AddFilter(p => p.CheckInDate < DateTime.Now.AddDays(-2));
    }
}