using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private IConfiguration configuration;
        public AuthController(IConfiguration configuration)
        {
            this.configuration= configuration;
        }

        [HttpPost]
        public IActionResult Authenticate([FromBody] Credential credential)
        {
            if (credential.UserName == "admin" && credential.Password == "admin")
            {
                // creating the security context
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email, "admin@admin.com"),
                    new Claim("Department","HR"),
                    new Claim("Admin","true"),
                    new Claim("Manager","true"),
                    new Claim("EmploymentDate", "2023-01-01")
                };

                var expiresAt = DateTime.Now.AddMinutes(20);

                return Ok(new
                {
                    access_token = CreateToken(claims,expiresAt),
                    expires_at = expiresAt
                });
            }

            ModelState.AddModelError("Unauthorized", "You are not authorized to see this endpoint");
            return Unauthorized(ModelState);
        }

        private string CreateToken(IEnumerable<Claim> claims, DateTime expiresAt)
        {
            var secretKey = Encoding.ASCII.GetBytes(configuration.GetValue<string>("SecretKey")?? "");

            //generate the jwt.
            var jwt = new JwtSecurityToken(
                claims: claims,
                notBefore: DateTime.Now,
                expires: expiresAt,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(secretKey),
                    SecurityAlgorithms.HmacSha256Signature)
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

    }


    public class Credential
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
