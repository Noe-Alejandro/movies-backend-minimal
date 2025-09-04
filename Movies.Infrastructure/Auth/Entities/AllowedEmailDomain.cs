namespace Movies.Infrastructure.Auth.Entities;

public sealed class AllowedEmailDomain
{
   public int Id { get; set; }
   public string Domain { get; set; } = "";
   public bool IsActive { get; set; } = true;
}
