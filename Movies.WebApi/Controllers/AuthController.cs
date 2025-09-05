using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Application.Abstractions.Service;
using Movies.Application.Common;
using Movies.Application.Exceptions;
using Movies.Application.Features.Auth.Dtos;

namespace Movies.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request, CancellationToken ct)
    {
        try
        {
            var result = await _authService.RegisterAsync(request, ct);
            return Ok(ResponseApiDto.Ok(result, "User registered successfully"));
        }
        catch (UserAlreadyExistsException ex)
        {
            return Conflict(ResponseApiDto.Fail<string>(ex.Message));
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(request, ct);
        return Ok(result);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request, CancellationToken ct)
    {
        var result = await _authService.RefreshTokenAsync(request, ct);
        return Ok(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] LogoutRequestDto request, CancellationToken ct)
    {
        await _authService.LogoutAsync(request, ct);
        return NoContent();
    }
}
