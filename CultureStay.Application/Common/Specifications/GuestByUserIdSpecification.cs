using CultureStay.Domain.Entities;
using CultureStay.Domain.Specification;

namespace CultureStay.Application.Common.Specifications;

public class GuestByUserIdSpecification : Specification<Guest>
{
    public GuestByUserIdSpecification(int userId)
    {
        AddFilter(g => g.UserId == userId);
    }
}