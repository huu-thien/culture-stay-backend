using CultureStay.Application.ViewModels.Review.Enums;
using CultureStay.Application.ViewModels.Review.QueryParameters;
using CultureStay.Domain.Entities;
using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.Review.Specifications;

public class GuestReviewSpecification : Specification<GuestReview>
{
    public GuestReviewSpecification(int guestId, ReviewQueryParameters? rqp = null)
    {
        AddInclude($"{nameof(Host)}.{nameof(User)}");
        AddFilter(gr => gr.GuestId == guestId);
        AddFilter(gr => gr.IsDeleted == false);
        
        if (rqp == null) return;
        
        if (rqp.OrderBy != null)
        {
            var orderBy = rqp.OrderBy switch {
                ReviewSortBy.Rating => nameof(GuestReview.Rating),
                ReviewSortBy.CreatedAt => nameof(GuestReview.CreatedAt),
                _ => nameof(GuestReview.CreatedAt)
            };
            
            AddOrderByField(orderBy);

            if (rqp.IsDescending)
                ApplyDescending();
        }
        
        ApplyPaging(rqp.PageIndex, rqp.PageSize);
    }
}