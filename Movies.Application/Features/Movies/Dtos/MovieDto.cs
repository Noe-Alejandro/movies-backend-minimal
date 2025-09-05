namespace Movies.Application.Features.Movies.Dtos;

public sealed record MovieDto(int Id, string Title, int Year, IReadOnlyList<string> Genres, int DurationMinutes, string? Synopsis);
