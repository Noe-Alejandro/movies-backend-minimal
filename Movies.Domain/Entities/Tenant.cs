using Movies.Domain.Entities.Auth;

namespace Movies.Domain.Entities;

public class Tenant
{
    public int Id { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public bool IsActive { get; set; }

    public ICollection<UserTenant> UserTenants { get; set; } = [];
}
