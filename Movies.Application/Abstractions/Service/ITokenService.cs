using Movies.Application.Features.Auth.Dtos;
using Movies.Domain.Entities.Auth;

namespace Movies.Application.Abstractions.Service;

public interface ITokenService
{
    AuthResultDto GenerateTokens(UserAuth user);
    string GenerateRefreshToken();
}
