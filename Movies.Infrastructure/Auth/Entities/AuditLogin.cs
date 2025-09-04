namespace Movies.Infrastructure.Auth.Entities;

public sealed class AuditLogin
{
   public long Id { get; set; }
   public int? UserId { get; set; }
   public int? ProviderId { get; set; }
   public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
   public string? Ip { get; set; }
   public string? UserAgent { get; set; }
   public string Result { get; set; } = "Success";
}
