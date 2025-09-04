using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Movies.WebApi.Endpoints
{
   public sealed class AuthEndpoints : IEndpointModule
   {
      public void MapEndpoints(IEndpointRouteBuilder app)
      {
         var group = app.MapGroup("/api/v1/auth")
             .WithTags("Auth");

         // Login fake (dev only)
         group.MapPost("/login", () =>
         {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, "noeshi.dev@gmail.com"),
                new Claim(ClaimTypes.Name, "Noe Gonzalez"),
                new Claim("perm", "movies.read"),
                new Claim("perm", "movies.write")
            };

            var config = app.ServiceProvider.GetRequiredService<IConfiguration>();

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);


            return Results.Ok(new
            {
               token = new JwtSecurityTokenHandler().WriteToken(token)
            });
         })
         .AllowAnonymous()
         .WithName("Auth_Login")
         .Produces(StatusCodes.Status200OK);
      }
   }
}
