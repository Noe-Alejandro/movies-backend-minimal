using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Abstractions.Repository;
using Movies.Application.Abstractions.Service;
using Movies.Infrastructure.Persistence;
using Movies.Infrastructure.Repositories;
using Movies.Infrastructure.Security;

namespace Movies.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var cs = config.GetConnectionString("Default");
        if (string.IsNullOrWhiteSpace(cs)) throw new InvalidOperationException("ConnectionStrings:Default missing.");

        services.AddDbContext<AppDbContext>(o => o.UseSqlServer(cs));

        // Repositories
        services.AddScoped<IMovieRepository, MovieRepository>();
        services.AddScoped<IAuthRepository, AuthRepository>();

        // Services
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}
