using MovieCatalog.Utility.ExceptionHandler.CustomExceptions;

namespace MovieCatalog.Utility.ExceptionHandler;

public class ExceptionHandler
{
    private readonly RequestDelegate _requestDelegate;

    public ExceptionHandler(RequestDelegate requestDelegate)
    {
        _requestDelegate = requestDelegate;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _requestDelegate(httpContext);
        }
        catch (AlreadyLogoutException ex)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsJsonAsync(new
            {
                Description = ex.Message
            });
        }
        catch (NotAuthorizedException ex)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsJsonAsync(new
            {
                Description = ex.Message
            });
        }
        catch (BadRequestException ex)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(new
            {
                Description = ex.Message
            });
        }
        catch (NotFoundException ex)
        {
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            await httpContext.Response.WriteAsJsonAsync(new
            {
                Description = ex.Message
            });
        }
        catch (ForbiddenException ex)
        {
            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            await httpContext.Response.WriteAsJsonAsync(new
            {
                Description = ex.Message
            });
        }
    }
}