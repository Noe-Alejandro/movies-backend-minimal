using Movies.Application.Common;
using Movies.Application.Features.Movies.Dtos;

namespace Movies.Application.Abstractions.Service;

public interface IMovieService
{
    Task<PagedResult<MovieDto>> GetMoviesAsync(GetMoviesRequestDto request, CancellationToken ct);
}
