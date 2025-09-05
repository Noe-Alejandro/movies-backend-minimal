namespace Movies.Domain.Entities.Auth;

public class Permission
{
   public int Id { get; set; }
   public string Key { get; set; } = default!;

   public string Name { get; set; } = default!;
   public string? Description { get; set; }
   public string? GroupName { get; set; }
   public ICollection<UserPermission> UserPermissions { get; set; } = [];
   public ICollection<RolePermission> RolePermissions { get; set; } = [];
}
