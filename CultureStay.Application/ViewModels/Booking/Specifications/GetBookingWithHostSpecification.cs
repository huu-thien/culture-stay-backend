using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.Booking.Specifications;

public class GetBookingWithHostSpecification : Specification<Domain.Entities.Booking>
{
    public GetBookingWithHostSpecification(int hostId)
    {
        AddFilter(b => b.Property.HostId == hostId);
    }
}