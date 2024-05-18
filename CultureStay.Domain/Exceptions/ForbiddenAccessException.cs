namespace CultureStay.Domain.Exceptions;

public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException(string? message = null) : base(
        message ?? "You do not have permission to access this resource.")
    {
    }
}