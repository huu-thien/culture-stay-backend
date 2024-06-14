using CultureStay.Domain.Enum;
using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.Cancellation.Specifications;

public class CancellationByIdSpecification : Specification<CancellationTicket>
{
    public CancellationByIdSpecification(int id)
    {
        AddInclude($"{nameof(Booking)}.{nameof(Property)}.{nameof(Host)}.{nameof(User)}");
        AddInclude($"{nameof(Booking)}.{nameof(Guest)}.{nameof(User)}");
        AddFilter(x => x.Id == id);
    }
}