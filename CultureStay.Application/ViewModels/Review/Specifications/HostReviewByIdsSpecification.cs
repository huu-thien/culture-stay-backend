using CultureStay.Domain.Entities;
using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.Review.Specifications;

public class HostReviewByIdsSpecification : Specification<HostReview>
{
    public HostReviewByIdsSpecification(int hostId, int guestId)
    {
        AddInclude($"{nameof(Guest)}.{nameof(User)}");
        AddFilter(hr => hr.HostId == hostId && hr.GuestId == guestId);
    }
}