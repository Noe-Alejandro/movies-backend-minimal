using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Movies.Application.Abstractions;
using System.Security.Claims;

namespace Movies.WebApi.Security
{
   public sealed class PermissionClaimsTransformation(IAuthRepository repo, IMemoryCache cache, ILogger<PermissionClaimsTransformation> log)
  : IClaimsTransformation
   {
      public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
      {
         if (principal.Identity is not ClaimsIdentity id || !id.IsAuthenticated) return principal;

         var uidStr = id.FindFirst("uid")?.Value;
         if (!int.TryParse(uidStr, out var uid)) return principal;

         var perms = await cache.GetOrCreateAsync("perms:" + uid, async e =>
         {
            e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return await repo.GetEffectivePermissionsAsync(uid, CancellationToken.None);
         });

         var existing = new HashSet<string>(id.FindAll("perm").Select(x => x.Value), StringComparer.OrdinalIgnoreCase);
         foreach (var p in perms)
            if (!existing.Contains(p)) id.AddClaim(new Claim("perm", p));

         return principal;
      }
   }
}
