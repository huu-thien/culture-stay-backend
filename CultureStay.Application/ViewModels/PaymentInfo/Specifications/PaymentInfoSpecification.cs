using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.PaymentInfo.Specifications;

public class PaymentInfoSpecification: Specification<Domain.Entities.PaymentInfo>
{
    public PaymentInfoSpecification(int hostId)
    {
        AddFilter(p => p.HostId == hostId);
        AddInclude(p => p.Host);
    }
}    
