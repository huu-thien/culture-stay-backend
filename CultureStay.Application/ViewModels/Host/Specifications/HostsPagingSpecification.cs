using CultureStay.Application.Common;
using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.Host.Specifications;

public class HostsPagingSpecification : Specification<Domain.Entities.Host>
{
    public HostsPagingSpecification(PagingParameters pp)
    {
        AddInclude(host => host.User);
        ApplyPaging(pp.PageIndex,pp.PageSize);
    }
}