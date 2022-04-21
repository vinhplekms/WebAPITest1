using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebAPITest2.Data;
using WebAPITest2.Enums;
using WebAPITest2.Models;
using WebAPITest2.Services;

namespace WebAPITest2.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IList<RoleEnum> _roles;

        public AuthorizeAttribute(params RoleEnum[] roles)
        {
            _roles = roles ?? new RoleEnum[] { };
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous) return;

            // authorization
            var user = (User)context.HttpContext.Items["User"];
            var role = (Role)context.HttpContext.Items["Role"];

            var ListStringRole = new List<string>();
            foreach (var roleEnum in _roles)
            {
                ListStringRole.Add(roleEnum.ToString());
            }

            if (user == null || (_roles.Any() && !ListStringRole.Contains(role.Name)))
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}
