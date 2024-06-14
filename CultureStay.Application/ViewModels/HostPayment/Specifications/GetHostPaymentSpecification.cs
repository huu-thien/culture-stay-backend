using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.HostPayment.Specifications;

public class GetHostPaymentSpecification : Specification<Domain.Entities.HostPayment>
{
    public GetHostPaymentSpecification(int hostPaymentInfo)
    {
        AddInclude(hp => hp.PaymentInfo.Host);
        AddFilter(hp => hp.Id == hostPaymentInfo);
    }
}