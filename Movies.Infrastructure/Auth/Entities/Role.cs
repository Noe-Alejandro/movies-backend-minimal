namespace Movies.Infrastructure.Auth.Entities;

public sealed class Role
{
   public int Id { get; set; }
   public string Name { get; set; } = "";
   public string? Description { get; set; }
   public bool IsActive { get; set; } = true;

   public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
   public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
