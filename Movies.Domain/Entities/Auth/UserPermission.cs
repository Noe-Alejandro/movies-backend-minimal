namespace Movies.Domain.Entities.Auth;

public class UserPermission
{
   public int UserId { get; set; }
   public int PermissionId { get; set; }
   public bool IsGranted { get; set; }

   public UserAuth User { get; set; } = default!;
   public Permission Permission { get; set; } = default!;
}
