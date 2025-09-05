namespace Movies.Application.Features.Auth.Dtos;

public class LoginRequestDto
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}
