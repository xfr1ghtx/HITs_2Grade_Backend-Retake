using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieCatalog.Models;
using MovieCatalog.Services;
using MovieCatalog.Storage;

namespace MovieCatalog.Controllers;

[Route("api/account")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IAuthService _authService;

    public AuthController(UserManager<User> userManager, IAuthService authService)
    {
        _userManager = userManager;
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterModel data)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var userWithSameData = await _userManager.Users.FirstOrDefaultAsync(y => y.Email == data.Email);

        if (userWithSameData != default)
        {
            return BadRequest(new
            {
                Description = "Пользователь с такой почтой уже существует"
            });
        }

        var user = new User
        {
            UserName = data.Username,
            Name = data.Name,
            Email = data.Email,
            BirthDate = data.BirthDate,
            Gender = data.Gender
        };

        var created = await _userManager.CreateAsync(user, data.Password);

        if (created.Succeeded)
        {
            var token = _authService.GenerateToken(user.Id);
            return Ok(new
            {
                Token = token
            });
        }
        else
        {
            return BadRequest(created.Errors);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel data)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var user = await _userManager.FindByNameAsync(data.Username);

        if (user == null)
        {
            return NotFound(new
            {
                Description = "Пользователь не существует"
            });
        }

        if (!await _userManager.CheckPasswordAsync(user, data.Password))
        {
            return BadRequest(new
            {
                Description = "Неверный пароль"
            });
        }

        var token = _authService.GenerateToken(user.Id);

        return Ok(new
        {
            Token = token
        });
    }

    [HttpPost("logout")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize(Policy = "LogoutToken")]
    public async Task<IActionResult> Logout()
    {
        var token = HttpContext.Request.Headers.Authorization;
        token = token.ToString().Replace("Bearer ", "");

        var expiredDate = new JwtSecurityTokenHandler().ReadJwtToken(token).ValidTo;

        var logoutToken = new Token
        {
            Value = token,
            ExpiredDate = expiredDate
        };

        await _authService.SaveLogoutToken(logoutToken);
        return Ok(new
        {
            Description = "Успешно"
        });
    }
}