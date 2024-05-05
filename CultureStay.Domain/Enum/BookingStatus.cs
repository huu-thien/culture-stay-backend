namespace CultureStay.Domain.Enum;

public enum BookingStatus
{
    Pending = 1,
    Confirmed,
    CheckedIn,
    Rejected,
    CancelledBeforeCheckIn,
    CancelledAfterCheckIn,
    Completed
}