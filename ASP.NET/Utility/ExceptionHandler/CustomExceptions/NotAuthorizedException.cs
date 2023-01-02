namespace MovieCatalog.Utility.ExceptionHandler.CustomExceptions;

public class NotAuthorizedException : Exception
{
    public NotAuthorizedException(string message) : base(message)
    {
    }
}