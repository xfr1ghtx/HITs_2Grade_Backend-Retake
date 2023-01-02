using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieCatalog.Services;

namespace MovieCatalog.Controllers;

[Route("api/favorites")]
[ApiController]
public class FavoriteMoviesController : ControllerBase
{
    private readonly IMoviesService _moviesService;

    public FavoriteMoviesController(IMoviesService favoriteMoviesService)
    {
        _moviesService = favoriteMoviesService;
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize(Policy = "LogoutToken")]
    public async Task<IActionResult> GetFavorites()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return Unauthorized(new
            {
                Description = "Пользователь не авторизирован"
            });
        }

        var movies = await _moviesService.GetFavoriteMovies(userId);

        return Ok(movies);
    }

    [HttpPost("{id:Guid}/add")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize(Policy = "LogoutToken")]
    public async Task<IActionResult> AddFavorites(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return Unauthorized(new
            {
                Description = "Пользователь не авторизирован"
            });
        }

        await _moviesService.AddFavorite(userId, id);

        return Ok(new
        {
            Description = "Успешно"
        });
    }

    [HttpDelete("{id:Guid}/delete")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize(Policy = "LogoutToken")]
    public async Task<IActionResult> DeleteFavorites(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return Unauthorized(new
            {
                Description = "Пользователь не авторизирован"
            });
        }

        await _moviesService.DeleteFavorite(userId, id);

        return Ok(new
        {
            Description = "Успешно"
        });
    }
}