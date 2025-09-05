using Movies.Application.Features.Movies.Dtos;
using Movies.Domain.Entities;

namespace Movies.Application.Features.Movies.Mappers;

public static class MovieMapper
{
    public static MovieDto ToDto(this Movie m)
    => new(
        m.Id,
        m.Title,
        m.Year,
        SplitGenres(m.Genres),
        m.DurationMinutes,
        m.Synopsis
    );

    private static IReadOnlyList<string> SplitGenres(string genres)
        => string.IsNullOrWhiteSpace(genres)
            ? Array.Empty<string>()
            : genres.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
