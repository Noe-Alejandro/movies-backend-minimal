using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Abstractions;
using Movies.Application.Features.Movies;

namespace Movies.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IMovieService, MovieService>();
            return services;
        }
    }
}
