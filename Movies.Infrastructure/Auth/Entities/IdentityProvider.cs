namespace Movies.Infrastructure.Auth.Entities;

public sealed class IdentityProvider
{
   public int Id { get; set; }
   public string Name { get; set; } = "";
   public string IssuerUri { get; set; } = "";

   public ICollection<UserIdentity> UserIdentities { get; set; } = new List<UserIdentity>();
}
