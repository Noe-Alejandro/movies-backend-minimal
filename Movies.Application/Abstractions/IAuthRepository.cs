namespace Movies.Application.Abstractions
{
    public interface IAuthRepository
    {
        Task<(int userId, int providerId)> EnsureUserAndIdentityAsync(
            string? issuer, string? subject, string? email, string? displayName, CancellationToken ct);

        Task TouchLoginAsync(int userId, int providerId, string? ip, string? userAgent, string result, CancellationToken ct);

        Task<bool> IsEmailDomainAllowedAsync(string? email, CancellationToken ct);

        Task<IReadOnlyList<string>> GetEffectivePermissionsAsync(int userId, CancellationToken ct);
    }
}
