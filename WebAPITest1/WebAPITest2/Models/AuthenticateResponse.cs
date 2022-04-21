using System.Text.Json.Serialization;
using WebAPITest2.Data;

namespace WebAPITest2.Models
{
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public string? Fullname { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? JwtToken { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string? RefreshToken { get; set; }

        public AuthenticateResponse(User user, string? jwtToken, string? refreshToken)
        {
            Id = user.Id;
            Fullname = user.Fullname;
            Username = user.Username;
            Email = user.Email;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}
