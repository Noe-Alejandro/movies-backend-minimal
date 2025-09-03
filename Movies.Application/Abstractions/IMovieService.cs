using Movies.Application.Features.Movies.Dtos;
using Movies.Application.Common;

namespace Movies.Application.Abstractions
{
   public interface IMovieService
   {
      Task<PagedResult<MovieDto>> GetMoviesAsync(GetMoviesRequestDto request, CancellationToken ct);
   }
}
