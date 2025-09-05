using Microsoft.Extensions.Logging;
using Movies.Application.Abstractions.Repository;
using Movies.Application.Abstractions.Service;
using Movies.Application.Common;
using Movies.Application.Features.Movies.Dtos;
using Movies.Application.Features.Movies.Mappers;

namespace Movies.Application.Features.Movies;

public sealed class MovieService(IMovieRepository repo, ILogger<MovieService> log) : IMovieService
{
    public async Task<PagedResult<MovieDto>> GetMoviesAsync(GetMoviesRequestDto request, CancellationToken ct)
    {
        log.LogInformation("GetMovies {@Req}", request);
        var (items, total) = await repo.SearchAsync(request, ct);
        var data = items.Select(x => x.ToDto()).ToList();
        return new PagedResult<MovieDto>(data, total, request.Page, request.PageSize);
    }
}
