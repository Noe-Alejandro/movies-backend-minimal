using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Application.Abstractions;
using Movies.Application.Features.Movies.Dtos;

namespace Movies.WebApi.Controllers
{
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
      [Authorize(Policy = "Movies.Read")]
      public async Task<IActionResult> GetAll([FromQuery] GetMoviesRequestDto request, CancellationToken ct)
      {
         var movies = await _movieService.GetMoviesAsync(request, ct);
         return Ok(movies);
      }
   }
}
