namespace Movies.Infrastructure.Auth.Entities;

public sealed class UserRole
{
   public int UserId { get; set; }
   public int RoleId { get; set; }

   public AuthUser User { get; set; } = default!;
   public Role Role { get; set; } = default!;
}
