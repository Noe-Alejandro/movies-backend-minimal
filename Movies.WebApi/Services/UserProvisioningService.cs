using Microsoft.IdentityModel.Tokens;
using Movies.Application.Abstractions;
using System.Security.Claims;

namespace Movies.WebApi.Services
{
   public sealed class UserProvisioningService(IAuthRepository repo, ILogger<UserProvisioningService> log)
  : IUserProvisioningService
   {
      public async Task EnsureUserAsync(ClaimsPrincipal principal, HttpContext http)
      {
         var iss = principal.FindFirst("iss")?.Value ?? principal.FindFirst("issuer")?.Value;
         var sub = principal.FindFirst("sub")?.Value;
         var email = principal.FindFirst("email")?.Value ?? principal.FindFirst("preferred_username")?.Value;
         var name = principal.FindFirst("name")?.Value ?? email ?? sub ?? "Unknown";

         if (!await repo.IsEmailDomainAllowedAsync(email, http.RequestAborted))
         {
            await repo.TouchLoginAsync(0, 0, http.Connection.RemoteIpAddress?.ToString(),
                                       http.Request.Headers.UserAgent.ToString(), "DeniedDomain", http.RequestAborted);
            throw new SecurityTokenValidationException("Email domain not allowed.");
         }

         var (userId, providerId) = await repo.EnsureUserAndIdentityAsync(iss, sub, email, name, http.RequestAborted);

         // claim "uid" para usar luego en transformaciones/consultas
         if (principal.Identity is ClaimsIdentity id && !id.HasClaim(c => c.Type == "uid"))
            id.AddClaim(new Claim("uid", userId.ToString()));

         await repo.TouchLoginAsync(userId, providerId,
             http.Connection.RemoteIpAddress?.ToString(),
             http.Request.Headers.UserAgent.ToString(),
             "Success", http.RequestAborted);
      }
   }
}
