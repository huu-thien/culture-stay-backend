using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.Wishlist.Specifications;

public class WishlistByGuestIdAndPropertyIdSpecification : Specification<Domain.Entities.Wishlist>
{
    public WishlistByGuestIdAndPropertyIdSpecification(int guestId, int propertyId, bool isDeleted = false)
    {
        AddFilter(w => w.GuestId == guestId);
        AddFilter(w => w.PropertyId == propertyId);

        if (isDeleted)
            ApplyIgnoreQueryFilters();
    }
}