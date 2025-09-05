using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Application.Abstractions.Service;
using Movies.Application.Features.Movies.Dtos;
using Movies.WebApi.Security;

namespace Movies.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.MoviesRead)]
    public async Task<IActionResult> GetAll([FromQuery] GetMoviesRequestDto request, CancellationToken ct)
    {
        var movies = await _movieService.GetMoviesAsync(request, ct);
        return Ok(movies);
    }
}
