using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Movies.Application.Abstractions.Repository;
using Movies.Application.Abstractions.Service;
using Movies.Application.Exceptions;
using Movies.Application.Features.Auth.Dtos;
using Movies.Domain.Entities.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Movies.Infrastructure.Security;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly IAuthRepository _authRepository;

    public TokenService(IConfiguration configuration, IAuthRepository authRepository)
    {
        _configuration = configuration;
        _authRepository = authRepository;
    }

    public AuthResultDto GenerateTokens(UserAuth user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");

        var secret = Environment.GetEnvironmentVariable("JWT_KEY")
                 ?? throw new MissingJwtKeyException();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim("displayName", user.DisplayName ?? string.Empty)
        };

        // ✅ Agregar roles como claims
        if (user.UserRoles != null)
        {
            foreach (var role in user.UserRoles.Select(r => r.Role.Name))
                claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // ✅ Agregar permisos efectivos como claims
        var perms = _authRepository.GetEffectivePermissionsAsync(user.Id, CancellationToken.None).Result;
        foreach (var perm in perms)
            claims.Add(new Claim("perm", perm));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var lifetimeSetting = jwtSettings["AccessTokenLifetimeMinutes"];
        if (!double.TryParse(lifetimeSetting, out var lifetimeMinutes))
            lifetimeMinutes = 15;

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(lifetimeMinutes),
            signingCredentials: creds
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        var refreshToken = GenerateRefreshToken();

        return new AuthResultDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = token.ValidTo
        };
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}
