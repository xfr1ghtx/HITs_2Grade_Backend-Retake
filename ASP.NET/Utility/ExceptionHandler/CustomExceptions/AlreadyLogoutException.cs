namespace MovieCatalog.Utility.ExceptionHandler.CustomExceptions;

public class AlreadyLogoutException : Exception
{
    public AlreadyLogoutException(string messsage) : base(messsage)
    {
    }
}