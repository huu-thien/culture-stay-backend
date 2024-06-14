using CultureStay.Domain.Entities;
using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.Payment.Specifications;

public class VnpHistoryGetByTxnRef : Specification<VnpHistory>
{
    public VnpHistoryGetByTxnRef(long txnRef)
    {
        AddInclude(p=>p.BookingPayment);
        AddFilter(p => p.vnp_TxnRef == txnRef);
    }
}