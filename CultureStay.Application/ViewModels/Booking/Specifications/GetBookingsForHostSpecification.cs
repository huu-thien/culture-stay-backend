using CultureStay.Application.ViewModels.Booking.QueryParameters;
using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.Booking.Specifications;

public class GetBookingsForHostSpecification : Specification<Domain.Entities.Booking>
{
    public GetBookingsForHostSpecification(int hostId, BookingQueryParameters bqp)
    {
        AddInclude(b => b.Property);
        AddInclude($"{nameof(Domain.Entities.Guest)}.{nameof(Domain.Entities.Guest.User)}");
        
        AddFilter(b => b.Property.HostId == hostId);
        if(bqp.Status is not null)
            AddFilter(b => b.Status == bqp.Status);

        if (bqp is { Search: not null })
        {
            AddSearchTerm(bqp.Search);
            AddSearchField("Guest.User.FullName");
            AddSearchField("Guest.User.Email");
            AddSearchField("Guest.User.PhoneNumber");
            AddSearchField("Property.Title");
        }
        
        AddOrderByField(bqp.OrderBy?.ToString() ?? nameof(Domain.Entities.Booking.CheckInDate));
        if (bqp.IsDescending)
            ApplyDescending();
        
        ApplyPaging(bqp.PageIndex, bqp.PageSize);
    }
}