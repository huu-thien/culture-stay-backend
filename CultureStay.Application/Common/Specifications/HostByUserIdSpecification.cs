using CultureStay.Domain.Entities;
using CultureStay.Domain.Specification;

namespace CultureStay.Application.Common.Specifications;

public class HostByUserIdSpecification : Specification<Host>
{
    public HostByUserIdSpecification(int userId)
    {
        AddFilter(h => h.UserId == userId);
        AddInclude(h => h.User);
    }
}