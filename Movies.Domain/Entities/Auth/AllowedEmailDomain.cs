namespace Movies.Domain.Entities.Auth;

public class AllowedEmailDomain
{
   public int Id { get; set; }
   public int TenantId { get; set; }
   public string Domain { get; set; } = default!;
   public bool IsActive { get; set; } = true;

   public Tenant Tenant { get; set; } = default!;
}
