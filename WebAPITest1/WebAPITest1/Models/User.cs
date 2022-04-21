using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPITest1.Models
{
    public class User
    {
       
        public int Id { get; set; }
      
        public string Username { get; set; }
       
        public string Password { get; set; }
       
        public string Email { get; set; }
       
        public string Fullname { get; set; }

      public IList<UserRole> UserRoles { get; set; }    
    }
}
