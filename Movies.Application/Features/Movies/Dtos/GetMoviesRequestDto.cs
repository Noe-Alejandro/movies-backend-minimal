namespace Movies.Application.Features.Movies.Dtos;

public sealed record GetMoviesRequestDto(string? Search, int? Year, int Page = 1, int PageSize = 20,
                                     int? YearFrom = null, int? YearTo = null, int? Decade = null);
