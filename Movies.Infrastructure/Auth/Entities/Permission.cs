namespace Movies.Infrastructure.Auth.Entities;

public sealed class Permission
{
   public int Id { get; set; }
   public string Key { get; set; } = "";       // e.g., "movies.read"
   public string Name { get; set; } = "";
   public string? Description { get; set; }
   public string? GroupName { get; set; }

   public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
   public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}
