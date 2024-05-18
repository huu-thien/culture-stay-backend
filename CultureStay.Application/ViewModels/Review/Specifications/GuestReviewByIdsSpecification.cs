using CultureStay.Domain.Entities;
using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.Review.Specifications;

public class GuestReviewByIdsSpecification : Specification<GuestReview>
{
    public GuestReviewByIdsSpecification(int guestId, int hostId)
    {
        AddFilter(gr => gr.GuestId == guestId && gr.HostId == hostId);
    }
}