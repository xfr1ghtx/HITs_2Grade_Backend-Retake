using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieCatalog.Models;
using MovieCatalog.Services;

namespace MovieCatalog.Controllers;

[Route("api/account/profile")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUsersService _usersService;

    public UserController(IUsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize(Policy = "LogoutToken")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return Unauthorized(new
            {
                Description = "Пользователь не авторизирован"
            });
        }

        var user = await _usersService.GetUserById(userId);
        var profileModel = new ProfileModel
        {
            Id = user.Id,
            Nickname = user.UserName,
            Email = user.Email,
            AvatarLink = user.Avatar,
            Name = user.Name,
            BirthDate = user.BirthDate,
            Gender = user.Gender
        };

        return Ok(profileModel);
    }

    [HttpPut]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize(Policy = "LogoutToken")]
    public async Task<IActionResult> ChangeProfile([FromBody] ProfileModel data)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return Unauthorized(new
            {
                Description = "Пользователь не авторизирован"
            });
        }

        await _usersService.Update(userId, data);

        return Ok(new
        {
            Description = "Успешно"
        });
    }
}