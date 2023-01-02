using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieCatalog.Services;

namespace MovieCatalog.Controllers;

[Route("api/movies")]
[ApiController]
public class MovieController : ControllerBase
{
    private readonly IMoviesService _moviesService;

    public MovieController(IMoviesService moviesService)
    {
        _moviesService = moviesService;
    }

    [HttpGet("{page:int}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize(Policy = "LogoutToken")]
    public async Task<IActionResult> GetMovies(int page = 1)
    {
        var movies = await _moviesService.GetMoviesOnPage(page);
        return Ok(movies);
    }

    [HttpGet("details/{id:Guid}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize(Policy = "LogoutToken")]
    public async Task<IActionResult> GetMoviesDetails(Guid id)
    {
        var movie = await _moviesService.GetMovieBy(id);
        return Ok(movie);
    }
}