namespace Movies.Infrastructure.Auth.Entities;

public sealed class UserIdentity
{
   public int Id { get; set; }
   public int UserId { get; set; }
   public int ProviderId { get; set; }
   public string Subject { get; set; } = ""; // JWT "sub"
   public string? EmailAtAuth { get; set; }
   public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
   public DateTime? LastSeenAt { get; set; }

   public AuthUser User { get; set; } = default!;
   public IdentityProvider Provider { get; set; } = default!;
}
