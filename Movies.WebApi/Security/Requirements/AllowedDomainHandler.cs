using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Movies.Application.Abstractions;
using System.Security.Claims;

namespace Movies.WebApi.Security.Requirements
{
   public sealed class AllowedDomainHandler(
    IAuthRepository repo,
    IMemoryCache cache,
    ILogger<AllowedDomainHandler> log) : AuthorizationHandler<AllowedDomainRequirement>
   {
      protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AllowedDomainRequirement requirement)
      {
         var email = context.User.FindFirst(ClaimTypes.Email)?.Value;
         if (!string.IsNullOrWhiteSpace(email) &&
             email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
         {
            context.Succeed(requirement);
         }
         return Task.CompletedTask;
      }
   }
}
