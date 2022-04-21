using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using WebAPITest2.Data;
using WebAPITest2.Helpers;
using WebAPITest2.Models;

namespace WebAPITest2.Services
{
    public class UserService : IUserService
    {
        private WebAPITest2Context _context;
        private IJwtUtils _jwtUtils;
        private readonly IConfiguration _configuration;

        public UserService(WebAPITest2Context context, IJwtUtils jwtUtils, IConfiguration configuration)
        {
            _context = context;
            _jwtUtils = jwtUtils;
            _configuration = configuration;
        }

    

        public async Task<AuthenticateResponse> Authenticate(UserDTOLogin userDTOLogin)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == userDTOLogin.Username);
            if (user == null)
            {
                return null;
            }

            if(!VerifyAccount(userDTOLogin.Password, user.Password, user.PasswordSalt))
            {
                return null;
            }

            
            var tokenModel = await _jwtUtils.GenerateJwtSecutiryToken(user);
          
            return new AuthenticateResponse(user, tokenModel.AccessToken, tokenModel.RefreshToken);
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
        public AuthenticateResponse RefreshToken(string token, string ipAddress)
        {
            throw new NotImplementedException();
        }

        public void RevokeToken(string token, string ipAddress)
        {
            throw new NotImplementedException();
        }

        public async Task<List<UserDTO>> FindAll()
        {
            return await _context.Users.Select(u => userToDTO(u)).ToListAsync();
        }

        public async Task<User> FindById(int Id)
        {
           return await _context.Users.FindAsync(Id);
        }

        public async Task<User> Create(User user, int roleId)
        {
            await _context.Users.AddAsync(user);
              await _context.SaveChangesAsync();

            await CreateUserRole(user.Id, roleId);
            await _context.SaveChangesAsync();

            return user;
        }

        private async Task CreateUserRole(int userId, int roleId)
        {
            var userRole = new UserRole
            {
                RoleId = roleId,
                UserId = userId
            };
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();
        }

        public async Task<User> Update(int id, UserDTO userDTO)
        {
            var user = _context.Users.Find(id);

            user.Username = userDTO.Username;
            user.Email = userDTO.Email;
            user.Fullname = userDTO.Fullname;

            _context.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

        }

        public bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
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
