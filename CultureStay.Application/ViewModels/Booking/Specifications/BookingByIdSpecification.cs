using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.Booking.Specifications;

public class BookingByIdSpecification : Specification<Domain.Entities.Booking>
{
    public BookingByIdSpecification(int id)
    {
        AddInclude($"{nameof(Domain.Entities.Booking.Property)}.{nameof(Domain.Entities.Property.Host)}.{nameof(Domain.Entities.User)}");
        AddInclude($"{nameof(Domain.Entities.Booking.Guest)}.{nameof(Domain.Entities.User)}");
        AddFilter(b => b.Id == id);
    }
}