using Movies.Domain.Entities.Auth;

namespace Movies.Application.Abstractions.Repository;

public interface IAuthRepository
{
    Task<UserAuth?> GetUserByEmailAsync(string email, CancellationToken ct);
    Task<UserAuth?> GetUserByIdAsync(int id, CancellationToken ct);
    Task AddUserAsync(UserAuth user, CancellationToken ct);

    Task SaveSessionAsync(int userId, string refreshToken, DateTime expiresAt, CancellationToken ct);
    Task<UserSession?> GetSessionByTokenAsync(string refreshToken, CancellationToken ct);
    Task RevokeSessionAsync(string refreshToken, bool allSessions, CancellationToken ct);

    Task<IEnumerable<string>> GetEffectivePermissionsAsync(int userId, CancellationToken ct);
    Task<bool> IsDomainAllowedAsync(string domain, CancellationToken ct);
}
