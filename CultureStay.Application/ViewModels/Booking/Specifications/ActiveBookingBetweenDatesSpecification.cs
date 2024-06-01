using CultureStay.Domain.Enum;
using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.Booking.Specifications;

public class ActiveBookingBetweenDatesSpecification : Specification<Domain.Entities.Booking>
{
    public ActiveBookingBetweenDatesSpecification(int propertyId, DateTime fromDate, DateTime toDate)
    {
        // Lay nhung booking dang active trong khoang tu fromDate den toDate
        // Ngay Checkout cua booking nay co the la ngay Checkin cua
        AddFilter(b => b.PropertyId == propertyId);
        AddFilter(b => b.Status != BookingStatus.Rejected && b.Status != BookingStatus.CancelledAfterCheckIn 
                                                          && b.Status != BookingStatus.Completed && b.Status != BookingStatus.CancelledBeforeCheckIn);
        AddFilter(b => (b.CheckOutDate.Date <= toDate.Date && b.CheckOutDate.Date > fromDate.Date) || 
                       (b.CheckInDate.Date <= toDate.Date && b.CheckInDate.Date >= fromDate.Date));
    }
}