namespace CultureStay.Domain.Exceptions.BookingException;

public class BookingCancellationException : BadInputException
{
    public BookingCancellationException(string message) : base(message)
    {
    }
}