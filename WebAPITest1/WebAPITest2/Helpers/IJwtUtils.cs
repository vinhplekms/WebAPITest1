using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using WebAPITest2.Data;
using WebAPITest2.Models;

namespace WebAPITest2.Helpers
{
    public interface IJwtUtils
    {
        public Task<TokenModel> GenerateJwtSecutiryToken(User user);
        public int? ValidateJwtToken(string token);
        public Task<string> GenerateRefresheToken(User user, string jwtId);
        public Task<TokenModel> RenewToken(TokenModel tokenModel);
        public bool CheckTokenExpired(string token);

    }
}
