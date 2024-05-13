using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.Property.Specifications;

public class PropertiesByIdsSpecification : Specification<Domain.Entities.Property>
{
    public PropertiesByIdsSpecification(List<int> ids, int pageIndex, int pageSize)
    {
        AddInclude(p => p.Host.User);
        AddInclude(p => p.PropertyImages);
        AddInclude(p => p.PropertyUtilities);
        AddInclude(p => p.PropertyReviews);
        AddInclude(p => p.Wishlists);
        AddFilter(p => ids.Contains(p.Id));
        ApplyPaging(pageIndex: pageIndex, pageSize: pageSize);
    }
}