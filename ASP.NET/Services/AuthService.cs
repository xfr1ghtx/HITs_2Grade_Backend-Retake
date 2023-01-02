using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MovieCatalog.Storage;
using MovieCatalog.Utility.ExceptionHandler.CustomExceptions;

namespace MovieCatalog.Services;

public interface IAuthService
{
    string GenerateToken(string userId);
    public Task SaveLogoutToken(Token token);
}

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _db;

    public AuthService(ApplicationDbContext db)
    {
        _db = db;
    }

    public string GenerateToken(string userId)
    {
        var claims = new List<Claim> {new(ClaimTypes.NameIdentifier, userId)};
        var token = new JwtSecurityToken(
            JwtConfigurations.Issuer,
            JwtConfigurations.Audience,
            notBefore: DateTime.UtcNow,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(JwtConfigurations.Lifetime)),
            signingCredentials: new SigningCredentials(JwtConfigurations.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256)
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task SaveLogoutToken(Token token)
    {
        await _db.LogoutTokens.AddAsync(token);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new NotAuthorizedException("Пользователь не авторизирован");
        }
    }
}