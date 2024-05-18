using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.Guest.Specifications;

public class GuestDetailSpecification : Specification<Domain.Entities.Guest>
{
    public GuestDetailSpecification(int id)
    {
        AddInclude(guest => guest.User);
        AddFilter(guest => guest.Id == id);
    }
}