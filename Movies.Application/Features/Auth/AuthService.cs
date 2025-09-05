using Movies.Application.Abstractions.Repository;
using Movies.Application.Abstractions.Service;
using Movies.Application.Exceptions;
using Movies.Application.Features.Auth.Dtos;
using Movies.Application.Features.Auth.Mappers;
using System.Security.Cryptography;
using System.Text;

namespace Movies.Application.Features.Auth;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly ITokenService _tokenService;

    public AuthService(IAuthRepository authRepository, ITokenService tokenService)
    {
        _authRepository = authRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResultDto> RegisterAsync(RegisterRequestDto request, CancellationToken ct)
    {
        var userFound = await _authRepository.GetUserByEmailAsync(request.Email, ct);
        if (userFound is not null)
        {
            throw new UserAlreadyExistsException(request.Email);
        }

        var passwordHash = HashPassword(request.Password);

        var user = AuthMapper.ToUserAuth(request, passwordHash);
        await _authRepository.AddUserAsync(user, ct);

        var tokens = _tokenService.GenerateTokens(user);
        await _authRepository.SaveSessionAsync(user.Id, tokens.RefreshToken, tokens.ExpiresAt, ct);

        return tokens;
    }

    public async Task<AuthResultDto> LoginAsync(LoginRequestDto request, CancellationToken ct)
    {
        var user = await _authRepository.GetUserByEmailAsync(request.Email, ct);
        if (user == null || !VerifyPassword(request.Password, user.PasswordHash!))
            throw new UnauthorizedAccessException("Invalid credentials");

        var tokens = _tokenService.GenerateTokens(user);
        await _authRepository.SaveSessionAsync(user.Id, tokens.RefreshToken, tokens.ExpiresAt, ct);

        return tokens;
    }

    public async Task<AuthResultDto> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken ct)
    {
        var session = await _authRepository.GetSessionByTokenAsync(request.RefreshToken, ct);
        if (session == null || session.ExpiresAt < DateTime.UtcNow || session.RevokedAt != null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        var user = await _authRepository.GetUserByIdAsync(session.UserId, ct);
        var tokens = _tokenService.GenerateTokens(user);

        await _authRepository.SaveSessionAsync(user.Id, tokens.RefreshToken, tokens.ExpiresAt, ct);
        return tokens;
    }

    public async Task LogoutAsync(LogoutRequestDto request, CancellationToken ct)
    {
        await _authRepository.RevokeSessionAsync(request.RefreshToken, request.AllSessions, ct);
    }

    private static byte[] HashPassword(string password)
    {
        using var sha = SHA256.Create();
        return sha.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    private static bool VerifyPassword(string password, byte[] storedHash)
    {
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return hash.SequenceEqual(storedHash);
    }
}