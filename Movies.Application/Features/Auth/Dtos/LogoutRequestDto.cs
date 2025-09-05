namespace Movies.Application.Features.Auth.Dtos;

public class LogoutRequestDto
{
    public string RefreshToken { get; set; } = default!;
    public bool AllSessions { get; set; }
}
