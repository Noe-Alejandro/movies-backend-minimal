using System.Security.Claims;

namespace Movies.WebApi.Services
{
   public interface IUserProvisioningService
   {
      Task EnsureUserAsync(ClaimsPrincipal principal, HttpContext http);
   }
}
