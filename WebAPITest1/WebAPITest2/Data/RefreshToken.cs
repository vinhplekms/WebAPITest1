
namespace WebAPITest2.Data
{
    public class RefreshToken
    {
        public string Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }  
        public string Token { get; set; }
        public string JwtId { get; set; }
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime ExpiredAt { get; set; }
    }
}
