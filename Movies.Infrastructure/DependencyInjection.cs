using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Abstractions;
using Movies.Infrastructure.Persistence;
using Movies.Infrastructure.Repositories;

namespace Movies.Infrastructure
{
   public static class DependencyInjection
   {
      public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
      {
         var cs = config.GetConnectionString("Default");
         if (string.IsNullOrWhiteSpace(cs)) throw new InvalidOperationException("ConnectionStrings:Default missing.");

         services.AddDbContext<AppDbContext>(o => o.UseSqlServer(cs));
         services.AddScoped<IMovieRepository, MovieRepository>();
         services.AddScoped<IAuthRepository, AuthRepository>();
         return services;
      }
   }
}
