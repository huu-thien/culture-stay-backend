using CultureStay.Domain.Entities;
using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.Review.Specifications;

public class PropertyReviewByIdsSpecification : Specification<PropertyReview>
{
    public PropertyReviewByIdsSpecification(int propertyId, int reviewerId)
    {
        AddFilter(pr => pr.PropertyId == propertyId && pr.GuestId == reviewerId);
    }
}