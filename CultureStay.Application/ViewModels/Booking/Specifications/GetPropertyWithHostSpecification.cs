using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.Booking.Specifications;

public class GetPropertyWithHostSpecification : Specification<Domain.Entities.Property>
{
    public GetPropertyWithHostSpecification(int propertyId)
    {
        AddInclude(p => p.Host);
        AddFilter(p => p.Id == propertyId);
    }
}