using CultureStay.Application.ViewModels.Booking.QueryParameters;
using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.Booking.Specifications;

public class GetBookingsForGuestSpecification : Specification<Domain.Entities.Booking>
{
    public GetBookingsForGuestSpecification(int guestId, BookingQueryParameters bqp)
    {
        AddInclude(b => b.Property);
        AddInclude($"{nameof(Domain.Entities.Property)}.{nameof(Domain.Entities.Host)}.{nameof(Domain.Entities.Property.Host.User)}");
        
        AddFilter(b => b.GuestId == guestId);
        
        if (bqp.Status is not null)
            AddFilter(b => b.Status == bqp.Status);

        if (bqp is { Search: not null })
        {
            AddSearchTerm(bqp.Search);
            AddSearchField("Property.Host.User.FullName");
            AddSearchField("Property.Host.User.Email");
            AddSearchField("Property.Host.User.PhoneNumber");
            AddSearchField("Property.Title");
        }
        
        AddOrderByField(bqp.OrderBy?.ToString() ?? nameof(Domain.Entities.Booking.CheckInDate));
        if (bqp.IsDescending)
            ApplyDescending();
        
        ApplyPaging(bqp.PageIndex, bqp.PageSize);
    }
}