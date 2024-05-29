namespace CultureStay.Domain.Exceptions.BookingException;

public class GuestIsHostException : BadInputException
{
    public GuestIsHostException(string message) : base(message)
    {
    }
}