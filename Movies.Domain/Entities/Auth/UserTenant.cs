namespace Movies.Domain.Entities.Auth;

public class UserTenant
{
    public int UserId { get; set; }
    public int TenantId { get; set; }

    public UserAuth User { get; set; } = default!;
    public Tenant Tenant { get; set; } = default!;
}
