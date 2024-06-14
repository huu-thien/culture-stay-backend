using CultureStay.Domain.Entities;
using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.Payment.Specifications;

public class BookingPaymentByBookingIdSpecification : Specification<BookingPayment>
{
    public BookingPaymentByBookingIdSpecification(int bookingId)
    {
        AddFilter(x => x.BookingId == bookingId);
    } 
}