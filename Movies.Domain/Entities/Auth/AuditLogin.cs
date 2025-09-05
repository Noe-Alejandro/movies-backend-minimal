namespace Movies.Domain.Entities.Auth;

public class AuditLogin
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string Email { get; set; } = default!;
    public DateTime AttemptedAt { get; set; }
    public bool Success { get; set; }
    public string? IpAddress { get; set; }
}
