using CultureStay.Application.ViewModels.HostPayment.QueryParameters;
using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.HostPayment.Specifications;

public class GetHostPaymentsSpecification:Specification<Domain.Entities.HostPayment>
{
    public GetHostPaymentsSpecification(HostPaymentQueryParameters hqp,int? hostId = null)
    {
        AddInclude(hp => hp.PaymentInfo.Host);
        
        if(hqp.Status is not null)
            AddFilter(h => h.Status == hqp.Status);
        
        if (hqp.IsDescending)
            ApplyDescending();
        
        if (hostId is not null)
            AddFilter(h => h.PaymentInfo.HostId == hostId);
        
        ApplyPaging(hqp.PageIndex, hqp.PageSize);
    }
}