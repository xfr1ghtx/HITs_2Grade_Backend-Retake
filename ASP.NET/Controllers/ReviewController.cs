using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieCatalog.Models;
using MovieCatalog.Services;

namespace MovieCatalog.Controllers;

[Route("api/movie/{movieId:Guid}/review")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IMoviesService _moviesService;

    public ReviewController(IMoviesService moviesService)
    {
        _moviesService = moviesService;
    }

    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize(Policy = "LogoutToken")]
    [HttpPost("add")]
    public async Task<IActionResult> AddReview([FromBody] ReviewModifyModel data, Guid movieId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return Unauthorized(new
            {
                Description = "Пользователь не авторизирован"
            });
        }

        await _moviesService.AddReview(data, movieId, userId);

        return Ok(new
        {
            Description = "Успешно"
        });
    }

    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize(Policy = "LogoutToken")]
    [HttpPut("{id:Guid}/edit")]
    public async Task<IActionResult> UpdateReview([FromBody] ReviewModifyModel data, Guid movieId, Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return Unauthorized(new
            {
                Description = "Пользователь не авторизирован"
            });
        }
        
        await _moviesService.UpdateReview(data, movieId, id, userId);

        return Ok(new
        {
            Description = "Успешно"
        });
    }

    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize(Policy = "LogoutToken")]
    [HttpDelete("{id:Guid}/delete")]
    public async Task<IActionResult> DeleteReview(Guid movieId, Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return Unauthorized(new
            {
                Description = "Пользователь не авторизирован"
            });
        }
        
        await _moviesService.DeleteReview(movieId, id, userId);

        return Ok(new
        {
            Description = "Успешно"
        });
    }
}