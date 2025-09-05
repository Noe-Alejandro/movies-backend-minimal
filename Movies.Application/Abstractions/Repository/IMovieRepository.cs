using Movies.Application.Features.Movies.Dtos;
using Movies.Domain.Entities;

namespace Movies.Application.Abstractions.Repository;

public interface IMovieRepository
{
    Task<(IReadOnlyList<Movie> Items, int Total)> SearchAsync(GetMoviesRequestDto request, CancellationToken ct);
}
