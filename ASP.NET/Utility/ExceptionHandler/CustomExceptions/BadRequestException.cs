namespace MovieCatalog.Utility.ExceptionHandler.CustomExceptions;

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {
    }
}