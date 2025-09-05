using Movies.Application.Features.Auth.Dtos;

namespace Movies.Application.Abstractions.Service;

public interface IAuthService
{
    Task<AuthResultDto> RegisterAsync(RegisterRequestDto request, CancellationToken ct);
    Task<AuthResultDto> LoginAsync(LoginRequestDto request, CancellationToken ct);
    Task<AuthResultDto> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken ct);
    Task LogoutAsync(LogoutRequestDto request, CancellationToken ct);
}
