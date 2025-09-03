using Movies.Application.Abstractions;
using Movies.Application.Common;
using Movies.Application.Features.Movies.Dtos;

namespace Movies.WebApi.Endpoints
{
   public sealed class MoviesEndpoints : IEndpointModule
   {
      public void MapEndpoints(IEndpointRouteBuilder app)
      {
         var group = app.MapGroup("/api/v1/movies")
                        .WithTags("Movies")
                        .RequireAuthorization("Movies.Read");

         group.MapGet("/", async (
             [AsParameters] GetMoviesRequestDto request,
             IMovieService service,
             CancellationToken ct) =>
         {
            var result = await service.GetMoviesAsync(request, ct);
            return Results.Ok(result);
         })
         .WithName("Movies_Get")
         .Produces<PagedResult<MovieDto>>(StatusCodes.Status200OK);
      }
   }
}
