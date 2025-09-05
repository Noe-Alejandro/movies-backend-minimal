using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Movies.WebApi.Security.Requirements;
using System.Text;

namespace Movies.WebApi.Security;

public static class JwtAuthExtensions
{
    public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration config)
    {
        services.AddMemoryCache();

        var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]!);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = config["Jwt:Issuer"],

                    ValidateAudience = true,
                    ValidAudience = config["Jwt:Audience"],

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    ValidateLifetime = true
                };
            });

        services.AddAuthorization(o =>
        {
            o.AddPolicy(AuthorizationPolicies.AllowedDomain, p => p.AddRequirements(new AllowedDomainRequirement()));
            o.AddPolicy(AuthorizationPolicies.MoviesRead, p => p.RequireClaim("perm", AuthorizationPolicies.MoviesRead));
            o.AddPolicy(AuthorizationPolicies.MoviesWrite, p => p.RequireClaim("perm", AuthorizationPolicies.MoviesWrite));
            o.AddPolicy(AuthorizationPolicies.MoviesDelete, p => p.RequireClaim("perm", AuthorizationPolicies.MoviesDelete));
        });

        services.AddScoped<IAuthorizationHandler, AllowedDomainHandler>();

        return services;
    }
}