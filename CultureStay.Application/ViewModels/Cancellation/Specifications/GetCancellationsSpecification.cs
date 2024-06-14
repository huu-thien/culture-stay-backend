using CultureStay.Application.ViewModels.Cancellation.QueryParameters;
using CultureStay.Domain.Enum;
using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.Cancellation.Specifications;

public class GetCancellationsSpecification : Specification<CancellationTicket>
{
    public GetCancellationsSpecification(CancellationTicketQueryParameters parameters)
    {
        
        if (parameters.Status.HasValue)
            AddFilter(x => x.Status == parameters.Status.Value);    
        
        if (parameters.IsGuest.HasValue)
            AddFilter(x => x.IsIssuerGuest == parameters.IsGuest.Value);
        
        if (parameters.IssuerId.HasValue)
            AddFilter(x => x.CreatedBy == parameters.IssuerId.Value);

        if (parameters.HostId.HasValue)
        {
            if (parameters.IsGuest.HasValue)
            {
                if (parameters.IsGuest.Value)
                    AddFilter(t => t.TheOtherPartyId == parameters.HostId.Value);
                else
                    AddFilter(t => t.CreatedBy == parameters.HostId.Value);
            }
            else
            {
                AddFilter(t => t.CreatedBy == parameters.HostId.Value || t.TheOtherPartyId == parameters.HostId.Value);
            }
        }
        
        AddOrderByField(parameters.OrderBy?.ToString() ?? nameof(CancellationTicket.Id));
        if (parameters.IsDescending)
            ApplyDescending();
        
        ApplyPaging(parameters.PageIndex, parameters.PageSize);
    } 
}