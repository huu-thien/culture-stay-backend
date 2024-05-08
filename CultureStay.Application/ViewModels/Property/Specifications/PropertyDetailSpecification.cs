using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.Property.Specifications;

public class PropertyDetailSpecification : Specification<Domain.Entities.Property>
{
    public PropertyDetailSpecification(int id)
    {
        AddInclude(p => p.PropertyImages);
        AddInclude(p => p.PropertyReviews);
        AddInclude(p => p.Host.User);
        AddInclude(p => p.Wishlists);
        AddFilter(p => p.Id == id);
    }
}