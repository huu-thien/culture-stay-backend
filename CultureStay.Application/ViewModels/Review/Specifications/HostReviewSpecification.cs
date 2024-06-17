using CultureStay.Application.ViewModels.Review.Enums;
using CultureStay.Application.ViewModels.Review.QueryParameters;
using CultureStay.Domain.Entities;
using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.Review.Specifications;

public class HostReviewSpecification : Specification<HostReview>
{
    public HostReviewSpecification(int hostId, ReviewQueryParameters? rqp = null)
    {
        AddInclude($"{nameof(Guest)}.{nameof(User)}");
        AddFilter(hr => hr.HostId == hostId);
        AddFilter(hr => hr.IsDeleted == false);
        
        if (rqp == null) return;
        
        if (rqp.OrderBy != null)
        {
            var orderBy = rqp.OrderBy switch {
                ReviewSortBy.Rating => nameof(HostReview.Rating),
                ReviewSortBy.CreatedAt => nameof(HostReview.CreatedAt),
                _ => nameof(HostReview.CreatedAt)
            };
            
            AddOrderByField(orderBy);

            if (rqp.IsDescending)
                ApplyDescending();
        } 
        
        ApplyPaging(rqp.PageIndex, rqp.PageSize);
    }
}