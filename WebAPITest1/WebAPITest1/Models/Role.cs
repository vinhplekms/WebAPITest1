using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPITest1.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<UserRole> UserRoles { get; set; }
    }
}
