namespace MovieCatalog.Utility.ExceptionHandler;

public static class MiddlewareExtension
{
    public static void UseExceptionHandlingMiddlewares(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandler>();
    }
}