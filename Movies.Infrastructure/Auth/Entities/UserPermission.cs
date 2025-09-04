namespace Movies.Infrastructure.Auth.Entities;

public sealed class UserPermission
{
   public int UserId { get; set; }
   public int PermissionId { get; set; }
   public bool IsGranted { get; set; } // true: grant, false: explicit deny

   public AuthUser User { get; set; } = default!;
   public Permission Permission { get; set; } = default!;
}
