using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using WebAPITest2.Data;
using WebAPITest2.Enums;
using WebAPITest2.Helpers;
using WebAPITest2.Models;
using WebAPITest2.Services;

namespace WebAPITest2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly WebAPITest2Context _context;
        private readonly IUserService userService;
        private readonly ICookieUtils cookieUtils;
        public UsersController(WebAPITest2Context context, IUserService userService, ICookieUtils cookieUtils)
        {
            _context = context;
            this.userService = userService;
            this.cookieUtils = cookieUtils;
        }

        // GET: api/Users
        [Authorize(RoleEnum.User)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            return await userService.FindAll();
        }

        // GET: api/Users/5
        [Authorize(RoleEnum.Admin, RoleEnum.User)]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await userService.FindById(id);

            if (user == null)
            {
                return NotFound();
            }

            return userToDTO(user);
        }

        // PUT: api/Users/5
        [Authorize(RoleEnum.Admin, RoleEnum.User)]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UserDTO userDTO)
        {
            if (id != userDTO.Id)
            {
                return Ok(new ErrorDetails
                {
                    StatusCode = 400,
                    Message = "Bad Request"
                });
            }
            if (!userService.UserExists(id))
            {
                return Ok(new ErrorDetails
                {
                    StatusCode = 404,
                    Message = "Not Found"
                });
            }

            await userService.Update(id, userDTO);
            return Ok(new ErrorDetails
            {
                StatusCode = 200,
                Message = "Successfull"
            });
        }

        // POST: api/Users
        [Authorize(RoleEnum.Admin, RoleEnum.User)]
        [HttpPost("{roleId}")]
        public async Task<ActionResult<UserDTO>> PostUser(int roleId, UserDTO userDTO)
        {
            if (userDTO == null)
            {
                return Ok(new ErrorDetails
                {
                    StatusCode = 400,
                    Message = "Invalid User"
                });
            }
            if (_context.Users.Where(u => u.Username == userDTO.Username || u.Email == userDTO.Email).Any())
            {
                return BadRequest("User has already existed!");
            }
            CreatePasswordHash(userDTO.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Username = userDTO.Username,
                Password = passwordHash,
                PasswordSalt = passwordSalt,
                Email = userDTO.Email,
                Fullname = userDTO.Fullname
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userToDTO(user));
        }
        // DELETE: api/Users/5
        [Authorize(RoleEnum.Admin, RoleEnum.User)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await userService.FindById(id);
            if (user == null)
            {
                return NotFound();
            }
            await userService.Delete(id);
            return NoContent();
        }

        [Authorize(RoleEnum.Admin, RoleEnum.User)]
        [HttpPost("register/{roleId}"), AllowAnonymous]
        public async Task<ActionResult<UserDTO>> Register(int roleId, UserDTO userDTO)
        {
            if (userDTO == null)
            {
                return BadRequest("No Value!");
            }
            if (_context.Users.Where(u => u.Username == userDTO.Username || u.Email == userDTO.Email).Any())
            {
                return BadRequest("User has already existed!");
            }
            CreatePasswordHash(userDTO.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Username = userDTO.Username,
                Password = passwordHash,
                PasswordSalt = passwordSalt,
                Email = userDTO.Email,
                Fullname = userDTO.Fullname
            };

            await userService.Create(user, roleId);

            return Ok(userToDTO(user));
        }

        [Authorize(RoleEnum.Admin, RoleEnum.User)]
        [HttpPost("login"), AllowAnonymous]
        public async Task<ActionResult> Login(UserDTOLogin request)
        {
            if (request == null)
            {
                return BadRequest("No value!");
            }

            var authenticate = await userService.Authenticate(request);
            if (authenticate == null) return Unauthorized();

            cookieUtils.SetTookenCookie(authenticate.JwtToken, "Access-Token", 7, Response);
            cookieUtils.SetTookenCookie(authenticate.RefreshToken, "Refresh-Token", 7, Response);

            return Ok("Token: " + authenticate.JwtToken);

        }

        private bool VerifyAccount(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                System.Diagnostics.Debug.WriteLine(Encoding.UTF8.GetString(computedHash));
                System.Diagnostics.Debug.WriteLine(Encoding.UTF8.GetString(passwordHash));
                return computedHash.SequenceEqual(passwordHash);

            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private static UserDTO userToDTO(User user) =>
            new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                Password = Encoding.UTF8.GetString(user.Password),
                Email = user.Email,
                Fullname = user.Fullname
            };


    }
}
