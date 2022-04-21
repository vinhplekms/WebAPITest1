using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebAPITest2.Data;
using WebAPITest2.Models;
using WebAPITest2.Services;

namespace WebAPITest2.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        //private readonly IList<Role> _roles;
        //private readonly IRoleService roleService;
        //public AuthorizeAttribute(IRoleService roleService, params Role[] roles)
        //{
        //    _roles = roles ?? new Role[] { };
        //    this.roleService = roleService;
        //}
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous) return;

            //authorization
            //var user = (UserDTO)context.HttpContext.Items["User"];
            //if (user == null || (_roles.Any() &&  !_roles.Contains(roleService.FindByUserId(user.Id))))
            //{
            //    context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            //}

            //====
            // authorization
            var user = context.HttpContext.Items["User"];
            if (user == null)
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };

        }
    }
}
