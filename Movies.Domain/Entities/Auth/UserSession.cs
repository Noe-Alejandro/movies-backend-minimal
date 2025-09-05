namespace Movies.Domain.Entities.Auth;

public class UserSession
{
   public Guid Id { get; set; }
   public int UserId { get; set; }
   public byte[] RefreshTokenHash { get; set; } = default!;
   public string? Device { get; set; }
   public string? Ip { get; set; }
   public string? UserAgent { get; set; }
   public DateTime CreatedAt { get; set; }
   public DateTime ExpiresAt { get; set; }
   public DateTime? RevokedAt { get; set; }

   public UserAuth User { get; set; } = default!;
}
