using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebAPITest2.Data;
using WebAPITest2.Models;

namespace WebAPITest2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly WebAPITest2Context _context;

        public AuthenController(IConfiguration configuration, WebAPITest2Context context)
        {
            _configuration = configuration;
            _context = context;
        }



        private string CreateToken(User user)
        {
            var role = _context.Roles.SingleOrDefault(r => r.Id == _context.UserRoles.SingleOrDefault(u => u.UserId == user.Id).RoleId);
            List<Claim> claims = new List<Claim>
            {
                    new Claim("Id", user.Id.ToString()),
                    new Claim("Username", user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Fullname),
                    // roles
                    new Claim("TokenId", Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Role, role.Name),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            Response.Cookies.Append("Access-Token", jwt, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict, Secure = true });
            Response.Cookies.Append("Access-Token", jwt, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict, Secure = true });
            return jwt;
        }



    }
}
