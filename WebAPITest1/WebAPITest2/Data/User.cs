using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPITest2.Data
{
    public class User
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public byte[]? Password { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public string? Email { get; set; }
        public string? Fullname { get; set; }
      public IList<UserRole>? UserRoles { get; set; }    
      public ICollection<RefreshToken>? RefreshTokens { get; set; }
    }
}
