using CultureStay.Application.Common;
using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.Wishlist.Specifications;

public class WishlistByGuestIdSpecification : Specification<Domain.Entities.Wishlist>
{
    public WishlistByGuestIdSpecification(PagingParameters pg, int guestId)
    {
        AddFilter(w => w.GuestId == guestId);
        AddFilter(w => w.IsDeleted == false);
        AddInclude(w => w.Property);
        AddInclude(w => w.Property.PropertyImages);
        AddInclude(w => w.Property.PropertyUtilities);
        AddInclude(w => w.Property.PropertyReviews);
        AddInclude(w => w.Property.Host.User);
        ApplyPaging(pageIndex: pg.PageIndex, pageSize: pg.PageSize);
    }
}