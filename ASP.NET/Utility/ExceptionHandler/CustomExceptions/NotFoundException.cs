namespace MovieCatalog.Utility.ExceptionHandler.CustomExceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}