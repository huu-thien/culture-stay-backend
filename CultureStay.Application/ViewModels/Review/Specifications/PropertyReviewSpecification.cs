using CultureStay.Application.ViewModels.Review.Enums;
using CultureStay.Application.ViewModels.Review.QueryParameters;
using CultureStay.Domain.Entities;
using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.Review.Specifications;

public class PropertyReviewSpecification : Specification<PropertyReview>
{
    public PropertyReviewSpecification(int propertyId, ReviewQueryParameters? qp = null)
    {
        AddInclude($"{nameof(Guest)}.{nameof(User)}");
        AddFilter(pr => pr.PropertyId == propertyId);
        
        if (qp == null) return;
        
        if (qp.OrderBy != null)
        {
            var orderBy = qp.OrderBy switch {
                ReviewSortBy.Rating => $"({nameof(PropertyReview.Cleanliness)} + {nameof(PropertyReview.Communication)} " +
                                       $"+ {nameof(PropertyReview.CheckIn)} + {nameof(PropertyReview.Accuracy)} " +
                                       $"+ {nameof(PropertyReview.Location)} + {nameof(PropertyReview.Value)}) / 6.0",
                ReviewSortBy.CreatedAt => nameof(HostReview.CreatedAt),
                _ => nameof(HostReview.CreatedAt)
            };
            
            AddOrderByField(orderBy);

            if (qp.IsDescending)
                ApplyDescending();
        } 
        
        ApplyPaging(qp.PageIndex, qp.PageSize);
    }
}