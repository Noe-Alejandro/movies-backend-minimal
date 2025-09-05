using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Abstractions.Service;
using Movies.Application.Features.Auth;
using Movies.Application.Features.Movies;

namespace Movies.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IMovieService, MovieService>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
