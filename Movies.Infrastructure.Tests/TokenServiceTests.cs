using Microsoft.Extensions.Configuration;
using Moq;
using Movies.Application.Abstractions.Repository;
using Movies.Domain.Entities.Auth;
using Movies.Infrastructure.Security;

namespace Movies.Infrastructure.Tests;
public class TokenServiceTests
{
    [Fact]
    public void GenerateTokens_ShouldReturnAuthResultDto()
    {
        // Arrange
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c.GetSection("Jwt")["Key"]).Returns("SuperSecretKey1234567890SuperSecretKey!!");
        mockConfig.Setup(c => c.GetSection("Jwt")["AccessTokenLifetimeInMinutes"]).Returns("60");
        mockConfig.Setup(c => c.GetSection("Jwt")["Issuer"]).Returns("movies-api");
        mockConfig.Setup(c => c.GetSection("Jwt")["Audience"]).Returns("movies-client");

        var mockAuthRepo = new Mock<IAuthRepository>();
        var service = new TokenService(mockConfig.Object, mockAuthRepo.Object);

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
        Assert.NotNull(result);
        Assert.NotEmpty(result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
    }
}