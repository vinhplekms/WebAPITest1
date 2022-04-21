using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebAPITest2.Data;
using WebAPITest2.Models;

namespace WebAPITest2.Helpers
{
    public class JwtUtils : IJwtUtils
    {
        private readonly WebAPITest2Context _context;
        private readonly IConfiguration _configuration;

        public JwtUtils(WebAPITest2Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<TokenModel> GenerateJwtSecutiryToken(User user)
        {
            var role = await _context.Roles.SingleOrDefaultAsync(r => r.Id == _context.UserRoles.SingleOrDefault(u => u.UserId == user.Id).RoleId);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                     new Claim("Id", user.Id.ToString()),
                    new Claim("Username", user.Username),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Fullname),
                    // roles
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Role, role.Name),
                }),
                Expires = DateTime.UtcNow.AddSeconds(10),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature)
            };


            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = await GenerateRefresheToken(user, securityToken.Id);

            return new TokenModel { AccessToken = tokenHandler.WriteToken(securityToken), RefreshToken = refreshToken };

        }


        public async Task<string> GenerateRefresheToken(User user, string jwtId)
        {
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                Token = GetUniqueToken(),
                JwtId = jwtId,
                IsUsed = false,
                IsRevoked = false,
                IssuedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddDays(7),
            };

            string GetUniqueToken()
            {
                var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
                var isTokenUnique = !_context.Users.Any(u => u.RefreshTokens.Any(t => t.Token == token));
                if (!isTokenUnique) return GetUniqueToken();

                return token;
            }

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken.Token;
        }

        public async Task<TokenModel> RenewToken(TokenModel tokenModel)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            try
            {
                // check accessToken valid
                var tokenInVerification = tokenHandler.ValidateToken(tokenModel.AccessToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,

                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = false
                }, out SecurityToken validatedToken); ;

                // Check alg
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);
                    if (!result)
                    {
                        return null; // Invalid Token
                    }
                }

                // Check accessToken expire
                var utcExpireDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
                if (expireDate > DateTime.UtcNow)
                {
                    return tokenModel;
                }

                // Check refreshToken exist in DB
                var storedToken = _context.RefreshTokens.FirstOrDefault(x => x.Token == tokenModel.RefreshToken);
                if (storedToken == null)
                {
                    return null; // RefreshToken does not exist
                }

                // Check refreshToken is used/revoked
                if (storedToken.IsUsed)
                {
                    return null; // RefreshToken is used 
                }

                if (storedToken.IsRevoked)
                {
                    return null; //RefreshToken is revoked
                }

                // Check accessToken id == Jwt in RefreshToken
                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if (storedToken.JwtId != jti)
                {
                    return null;
                     // token id does not match 
                }


                // Update Token
                storedToken.IsRevoked = true;
                storedToken.IsUsed = true;
                _context.Update(storedToken);
                await _context.SaveChangesAsync();

                // Create new Token 
                var user = _context.Users.SingleOrDefault(u => u.Id == storedToken.UserId);
                var token = await GenerateJwtSecutiryToken(user);
                return token; //renew token successfully
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong!");
            }
        }

        public bool CheckTokenExpired(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            try
            {
                // check accessToken valid
                var tokenInVerification = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,

                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = false
                }, out SecurityToken validatedToken);

                // Check accessToken expire
                var utcExpire = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expire = ConvertUnixTimeToDateTime(utcExpire);
                if (expire > DateTime.UtcNow)
                {
                    return true; // not expire
                }

            }
            catch (Exception ex)
            {
                return false;
            }

            return false;
        }
        private DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            .AddSeconds(utcExpireDate).ToUniversalTime();
            return dateTimeInterval;
        }

        public int? ValidateJwtToken(string token)
        {
            if (token == null) return null;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,

                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = false
                }, out SecurityToken validatedToken); ;

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "Id").Value);

                return userId;
            }
            catch (Exception)
            {
                return null;
            }
        }


    }
}
