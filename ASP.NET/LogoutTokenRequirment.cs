using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MovieCatalog.Utility.ExceptionHandler.CustomExceptions;
using MovieCatalog.Storage;


namespace MovieCatalog;

public class LogoutTokenRequirement : IAuthorizationRequirement
{
    public LogoutTokenRequirement()
    {
    }
}

public class LogoutTokenRequirementHandler : AuthorizationHandler<IAuthorizationRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IServiceScopeFactory _serviceScope;

    public LogoutTokenRequirementHandler(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory serviceScope)
    {
        _httpContextAccessor = httpContextAccessor;
        _serviceScope = serviceScope;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        IAuthorizationRequirement requirement)
    {
        if (_httpContextAccessor.HttpContext != null)
        {
            var token = _httpContextAccessor.HttpContext.Request.Headers.Authorization.FirstOrDefault();
            if (token == null)
            {
                throw new NotAuthorizedException("Пользователь не авторизован");
            }

            token = token.Replace("Bearer ", "");

            using (var scope = _serviceScope.CreateScope())
            {
                var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var tokenEntity = await _db.LogoutTokens.FirstOrDefaultAsync(x => x.Value == token);

                if (tokenEntity != default)
                {
                    throw new NotAuthorizedException("Не авторизованный");
                }

                context.Succeed(requirement);
            }
        }
        else
        {
            throw new BadRequestException("Не авторизованный");
        }
    }
}