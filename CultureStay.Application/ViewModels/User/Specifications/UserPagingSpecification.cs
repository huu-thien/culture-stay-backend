using CultureStay.Application.Common;
using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.User.Specifications;

public class UserPagingSpecification : Specification<Domain.Entities.User>
{
    public UserPagingSpecification(PagingParameters pp)
    {
        if (pp.IsDescending)
            ApplyDescending();
        ApplyPaging(pp.PageIndex, pp.PageSize);
    }
}