using Movies.Application.Features.Auth.Dtos;
using Movies.Domain.Entities.Auth;

namespace Movies.Application.Features.Auth.Mappers;

public static class AuthMapper
{
    public static AuthResultDto ToAuthResultDto(string accessToken, string refreshToken, DateTime expiresAt)
    {
        return new AuthResultDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt
        };
    }

    public static UserAuth ToUserAuth(RegisterRequestDto dto, byte[] passwordHash)
    {
        return new UserAuth
        {
            Email = dto.Email,
            DisplayName = dto.FullName ?? dto.Email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            EmailVerified = false
        };
    }
}
