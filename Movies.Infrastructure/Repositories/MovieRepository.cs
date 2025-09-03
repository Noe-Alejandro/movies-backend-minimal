using Microsoft.EntityFrameworkCore;
using Movies.Application.Abstractions;
using Movies.Application.Features.Movies.Dtos;
using Movies.Domain.Entities;
using Movies.Infrastructure.Persistence;

namespace Movies.Infrastructure.Repositories
{
   public sealed class MovieRepository(AppDbContext db) : IMovieRepository
   {
      public async Task<(IReadOnlyList<Movie> Items, int Total)> SearchAsync(GetMoviesRequestDto request, CancellationToken ct)
      {
         var q = db.Movies.AsNoTracking();

         if (!string.IsNullOrWhiteSpace(request.Search))
         {
            var t = request.Search.Trim();
            q = q.Where(m => EF.Functions.Like(m.Title, $"%{t}%"));
         }

         if (request.Year.HasValue) q = q.Where(m => m.Year == request.Year.Value);
         else if (request.Decade.HasValue) q = q.Where(m => m.Year >= request.Decade && m.Year <= request.Decade + 9);
         else if (request.YearFrom.HasValue && request.YearTo.HasValue) q = q.Where(m => m.Year >= request.YearFrom && m.Year <= request.YearTo);

         var total = await q.CountAsync(ct);
         var items = await q.OrderBy(m => m.Title)
                            .Skip((request.Page - 1) * request.PageSize)
                            .Take(request.PageSize)
                            .ToListAsync(ct);
         return (items, total);
      }
   }
}
