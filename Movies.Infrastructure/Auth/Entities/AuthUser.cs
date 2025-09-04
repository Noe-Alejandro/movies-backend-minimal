namespace Movies.Infrastructure.Auth.Entities;

public sealed class AuthUser
{
   public int Id { get; set; }
   public string? Email { get; set; }
   public string? DisplayName { get; set; }
   public bool IsActive { get; set; } = true;
   public bool EmailVerified { get; set; } = false;
   public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
   public DateTime? LastLoginAt { get; set; }
   public byte[]? PasswordHash { get; set; } // optional for local login

   public ICollection<UserIdentity> Identities { get; set; } = new List<UserIdentity>();
   public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
   public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}
