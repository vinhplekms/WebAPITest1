using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPITest2.Data
{
    public class Role
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public IList<UserRole>? UserRoles { get; set; }
    }
}
