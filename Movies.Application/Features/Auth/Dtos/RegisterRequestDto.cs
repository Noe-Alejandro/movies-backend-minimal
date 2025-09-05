namespace Movies.Application.Features.Auth.Dtos;

public class RegisterRequestDto
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string FullName { get; set; } = default!;
}
