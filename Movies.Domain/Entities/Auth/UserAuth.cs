namespace Movies.Domain.Entities.Auth;

public class UserAuth
{
   public int Id { get; set; }
   public string? Email { get; set; }
   public string? DisplayName { get; set; }
   public bool IsActive { get; set; } = true;
   public bool EmailVerified { get; set; } = false;
   public DateTime CreatedAt { get; set; }
   public DateTime? LastLoginAt { get; set; }
   public byte[]? PasswordHash { get; set; }

   public ICollection<UserTenant> UserTenants { get; set; } = [];
   public ICollection<UserRole> UserRoles { get; set; } = [];
   public ICollection<UserPermission> UserPermissions { get; set; } = [];
   public ICollection<UserSession> Sessions { get; set; } = [];
}
