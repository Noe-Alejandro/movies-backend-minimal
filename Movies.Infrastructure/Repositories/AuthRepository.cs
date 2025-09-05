using Microsoft.EntityFrameworkCore;
using Movies.Application.Abstractions.Repository;
using Movies.Domain.Entities.Auth;
using Movies.Infrastructure.Persistence;
using System.Security.Cryptography;
using System.Text;

namespace Movies.Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _db;

    public AuthRepository(AppDbContext db) => _db = db;

    public async Task<UserAuth?> GetUserByEmailAsync(string email, CancellationToken ct)
    {
        return await _db.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.UserPermissions).ThenInclude(up => up.Permission)
            .FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public async Task<UserAuth?> GetUserByIdAsync(int id, CancellationToken ct)
    {
        return await _db.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.UserPermissions).ThenInclude(up => up.Permission)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task AddUserAsync(UserAuth user, CancellationToken ct)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SaveSessionAsync(int userId, string refreshToken, DateTime expiresAt, CancellationToken ct)
    {
        var refreshTokenHash = HashToken(refreshToken);

        var session = new UserSession
        {
            UserId = userId,
            RefreshTokenHash = refreshTokenHash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt
        };

        _db.UserSessions.Add(session);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<UserSession?> GetSessionByTokenAsync(string refreshToken, CancellationToken ct)
    {
        var hash = HashToken(refreshToken);
        return await _db.UserSessions
            .FirstOrDefaultAsync(s => s.RefreshTokenHash == hash, ct);
    }

    public async Task RevokeSessionAsync(string refreshToken, bool allSessions, CancellationToken ct)
    {
        var hash = HashToken(refreshToken);
        var session = await _db.UserSessions.FirstOrDefaultAsync(s => s.RefreshTokenHash == hash, ct);

        if (session == null) return;

        if (allSessions)
        {
            var sessions = _db.UserSessions.Where(s => s.UserId == session.UserId);
            foreach (var s in sessions) s.RevokedAt = DateTime.UtcNow;
        }
        else
        {
            session.RevokedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<string>> GetEffectivePermissionsAsync(int userId, CancellationToken ct)
    {
        var rolePerms = await _db.UserRoles
            .Where(ur => ur.UserId == userId)
            .SelectMany(ur => ur.Role.RolePermissions.Select(rp => rp.Permission.Key))
            .ToListAsync(ct);

        var directPerms = await _db.UserPermissions
            .Where(up => up.UserId == userId && up.IsGranted)
            .Select(up => up.Permission.Key)
            .ToListAsync(ct);

        return rolePerms.Concat(directPerms).Distinct(StringComparer.OrdinalIgnoreCase);
    }

    public async Task<bool> IsDomainAllowedAsync(string domain, CancellationToken ct)
    {
        return await _db.AllowedEmailDomains
            .AnyAsync(d => d.Domain == domain && d.IsActive, ct)
            || await _db.AllowedEmailDomains
            .AnyAsync(d => d.Domain == "*", ct);
    }

    private static byte[] HashToken(string token)
    {
        using var sha = SHA256.Create();
        return sha.ComputeHash(Encoding.UTF8.GetBytes(token));
    }
}
