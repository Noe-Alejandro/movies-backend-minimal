using Microsoft.Extensions.Configuration;
using Moq;
using Movies.Application.Abstractions.Repository;
using Movies.Application.Exceptions;
using Movies.Domain.Entities.Auth;
using Movies.Infrastructure.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Movies.Infrastructure.Tests;
public class TokenServiceTests
{
    private const string SecretKey = "SuperSecretKey1234567890SuperSecretKey!!";

    private static IConfiguration GetConfig(string? lifetime = "60")
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"Jwt:Key", SecretKey},
            {"Jwt:AccessTokenLifetimeInMinutes", lifetime},
            {"Jwt:Issuer", "movies-api"},
            {"Jwt:Audience", "movies-client"}
        };
        return new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings!).Build();
    }

    [Fact]
    public void GenerateTokens_ShouldReturnTokens_WhenUserHasRolesAndPerms()
    {
        // Arrange
        Environment.SetEnvironmentVariable("JWT_KEY_API_MOVIES", SecretKey);

        var mockAuthRepo = new Mock<IAuthRepository>();
        mockAuthRepo.Setup(r => r.GetEffectivePermissionsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<string> { "perm1", "perm2" });

        var service = new TokenService(GetConfig(), mockAuthRepo.Object);
        var user = new UserAuth
        {
            Id = 1,
            Email = "test@test.com",
            DisplayName = "Tester",
            UserRoles = new List<UserRole> { new UserRole { Role = new Role { Name = "Admin" } } }
        };

        // Act
        var result = service.GenerateTokens(user);

        // Assert
        Assert.NotNull(result.AccessToken);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(result.AccessToken);

        // Verificar claims
        Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        Assert.Contains(jwt.Claims, c => c.Type == "perm" && c.Value == "perm1");
        Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.Email && c.Value == "test@test.com");
    }


    [Fact]
    public void GenerateTokens_ShouldHandleNullRoles()
    {
        Environment.SetEnvironmentVariable("JWT_KEY_API_MOVIES", SecretKey);

        var mockAuthRepo = new Mock<IAuthRepository>();
        mockAuthRepo.Setup(r => r.GetEffectivePermissionsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<string>());

        var service = new TokenService(GetConfig(), mockAuthRepo.Object);
        var user = new UserAuth { Id = 2, Email = "nobody@test.com" };

        var result = service.GenerateTokens(user);

        Assert.NotNull(result.AccessToken);
    }

    [Fact]
    public void GenerateTokens_ShouldFallbackToDefaultLifetime_WhenInvalidConfig()
    {
        Environment.SetEnvironmentVariable("JWT_KEY_API_MOVIES", SecretKey);

        var mockAuthRepo = new Mock<IAuthRepository>();
        mockAuthRepo.Setup(r => r.GetEffectivePermissionsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<string>());

        var service = new TokenService(GetConfig("INVALID"), mockAuthRepo.Object);
        var user = new UserAuth { Id = 3, Email = "bad@test.com" };

        var result = service.GenerateTokens(user);

        Assert.True(result.ExpiresAt <= DateTime.UtcNow.AddMinutes(16)); // cayó en 15 mins
    }

    [Fact]
    public void GenerateTokens_ShouldThrow_WhenSecretMissing()
    {
        Environment.SetEnvironmentVariable("JWT_KEY_API_MOVIES", null); // Limpia variable

        var service = new TokenService(GetConfig(), Mock.Of<IAuthRepository>());
        var user = new UserAuth { Id = 4, Email = "fail@test.com" };

        Assert.Throws<MissingJwtKeyException>(() => service.GenerateTokens(user));
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnBase64String()
    {
        var service = new TokenService(GetConfig(), Mock.Of<IAuthRepository>());

        var token = service.GenerateRefreshToken();

        Assert.False(string.IsNullOrEmpty(token));
        // Debe ser Base64
        Convert.FromBase64String(token);
    }
}
