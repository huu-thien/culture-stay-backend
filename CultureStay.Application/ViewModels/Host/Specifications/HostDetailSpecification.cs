using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.Host.Specifications;

public class HostDetailSpecification : Specification<Domain.Entities.Host>
{
    public HostDetailSpecification(int id)
    {
        AddInclude(host => host.User);
        AddFilter(host => host.Id == id);
    }
}