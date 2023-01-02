namespace MovieCatalog.Utility.ExceptionHandler.CustomExceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message)
    {
    }
}