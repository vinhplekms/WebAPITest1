namespace WebAPITest2.Data
{
    public class UserRole
    {
        public int UserId { get; set; }
        public User? User { get; set; } 
        public int RoleId { get; set; } 
        public Role? Role { get; set; }

    }
}
