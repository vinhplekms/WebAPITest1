using WebAPITest2.Helpers;
using WebAPITest2.Models;
using WebAPITest2.Services;

namespace WebAPITest2.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUserService userService, IRoleService roleService, IJwtUtils jwtUtils)
        {

            var accessToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last() ?? context.Request.Cookies["Access-Token"];
            var refreshToken = context.Request.Cookies["Refresh-Token"];

            // check accessToken is expired
            if (accessToken != null && refreshToken != null && !jwtUtils.CheckTokenExpired(accessToken))
            {
                var tokenModel = await jwtUtils.RenewToken(new TokenModel { AccessToken = accessToken, RefreshToken = refreshToken });
                if (tokenModel == null)
                {
                    accessToken = "";
                }
                else
                {
                    if (tokenModel != null) accessToken = tokenModel.AccessToken;
                }
            }
            var userId = jwtUtils.ValidateJwtToken(accessToken);
            if (userId != null)
            {
                context.Items["User"] = await userService.FindById(userId.Value);
                context.Items["Role"] = roleService.FindByUserId((int)userId);
            }
            await _next(context);
        }
    }
}
