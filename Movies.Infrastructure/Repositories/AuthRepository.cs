using Microsoft.EntityFrameworkCore;
using Movies.Application.Abstractions;
using Movies.Infrastructure.Auth.Entities;
using Movies.Infrastructure.Persistence;

namespace Movies.Infrastructure.Repositories
{
   public sealed class AuthRepository(AppDbContext db) : IAuthRepository
   {
      public async Task<(int userId, int providerId)> EnsureUserAndIdentityAsync(
          string? issuer, string? subject, string? email, string? displayName, CancellationToken ct)
      {
         // 1) Provider
         var provider = await db.IdentityProviders
           .FirstOrDefaultAsync(p => p.IssuerUri == issuer, ct)
           ?? db.IdentityProviders.Add(new IdentityProvider { Name = issuer ?? "Unknown", IssuerUri = issuer ?? "unknown" }).Entity;

         // 2) User (por email si existe)
         var user = !string.IsNullOrWhiteSpace(email)
           ? await db.AuthUsers.FirstOrDefaultAsync(u => u.Email == email, ct)
           : null;

         user ??= db.AuthUsers.Add(new AuthUser { Email = email, DisplayName = displayName, IsActive = true }).Entity;

         await db.SaveChangesAsync(ct);

         // 3) UserIdentity (iss+sub)
         var identity = await db.UserIdentities
           .FirstOrDefaultAsync(i => i.ProviderId == provider.Id && i.Subject == (subject ?? ""), ct);

         if (identity is null)
         {
            identity = new UserIdentity { UserId = user.Id, ProviderId = provider.Id, Subject = subject ?? "", EmailAtAuth = email };
            db.UserIdentities.Add(identity);
            await db.SaveChangesAsync(ct);
         }

         return (user.Id, provider.Id);
      }

      public async Task TouchLoginAsync(int userId, int providerId, string? ip, string? userAgent, string result, CancellationToken ct)
      {
         var user = await db.AuthUsers.FindAsync([userId], ct);
         if (user != null) user.LastLoginAt = DateTime.UtcNow;

         db.AuditLogins.Add(new AuditLogin
         {
            UserId = userId,
            ProviderId = providerId,
            OccurredAt = DateTime.UtcNow,
            Ip = ip,
            UserAgent = userAgent,
            Result = result
         });

         await db.SaveChangesAsync(ct);
      }

      public async Task<bool> IsEmailDomainAllowedAsync(string? email, CancellationToken ct)
      {
         if (string.IsNullOrWhiteSpace(email)) return false;
         var at = email.IndexOf('@');
         if (at < 0) return false;
         var domain = email[(at + 1)..].ToLowerInvariant();

         return await db.AllowedEmailDomains.AnyAsync(d => d.IsActive && d.Domain == domain, ct);
      }

      public async Task<IReadOnlyList<string>> GetEffectivePermissionsAsync(int userId, CancellationToken ct)
      {
         // Role permissions UNION (user grants) EXCEPT (user denies)
         var rolePerms = from ur in db.UserRoles
                         where ur.UserId == userId
                         join rp in db.RolePermissions on ur.RoleId equals rp.RoleId
                         join p in db.Permissions on rp.PermissionId equals p.Id
                         select p.Key;

         var userGrants = from up in db.UserPermissions
                          where up.UserId == userId && up.IsGranted
                          join p in db.Permissions on up.PermissionId equals p.Id
                          select p.Key;

         var userDenies = from up in db.UserPermissions
                          where up.UserId == userId && !up.IsGranted
                          join p in db.Permissions on up.PermissionId equals p.Id
                          select p.Key;

         var list = await rolePerms.Union(userGrants).Except(userDenies).Distinct().ToListAsync(ct);
         return list;
      }
   }
}
