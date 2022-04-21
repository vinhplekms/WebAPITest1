using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPITest2.Models
{
    public class RoleDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
