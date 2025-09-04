using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Movies.Application.Abstractions;
using Movies.WebApi.Security.Requirements;
using System.Security.Claims;
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

             options.Events = new JwtBearerEvents
             {
                OnTokenValidated = async ctx =>
                {
                   var repo = ctx.HttpContext.RequestServices.GetRequiredService<IAuthRepository>();

                   var issuer = ctx.Principal.FindFirst("iss")?.Value ?? options.TokenValidationParameters.ValidIssuer;
                   var subject = ctx.Principal.FindFirst("sub")?.Value;
                   var email = ctx.Principal.FindFirst(ClaimTypes.Email)?.Value;
                   var displayName = ctx.Principal.Identity?.Name ?? email;

                   var (userId, providerId) = await repo.EnsureUserAndIdentityAsync(
                       issuer, subject, email, displayName, ctx.HttpContext.RequestAborted);

                   await repo.TouchLoginAsync(
                       userId,
                       providerId,
                       ctx.HttpContext.Connection.RemoteIpAddress?.ToString(),
                       ctx.HttpContext.Request.Headers.UserAgent.ToString(),
                       "Success",
                       ctx.HttpContext.RequestAborted);

                   var perms = await repo.GetEffectivePermissionsAsync(userId, ctx.HttpContext.RequestAborted);
                   var identity = ctx.Principal.Identity as ClaimsIdentity;
                   foreach (var perm in perms)
                      identity?.AddClaim(new Claim("perm", perm));
                }
             };
          });

      services.AddAuthorization(o =>
      {
         o.AddPolicy("AllowedDomain", p => p.AddRequirements(new AllowedDomainRequirement()));
         o.AddPolicy("Movies.Read", p => p.RequireClaim("perm", "movies.read"));
         o.AddPolicy("Movies.Write", p => p.RequireClaim("perm", "movies.write"));
         o.AddPolicy("Movies.Delete", p => p.RequireClaim("perm", "movies.delete"));
      });

      services.AddScoped<IAuthorizationHandler, AllowedDomainHandler>();

      return services;
   }
}
