using Realms.Sync.Exceptions;
using System.Net;
using WebAPITest2.Models;

namespace WebAPITest2.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            this._next = next;
            this._logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);

                var response = context.Response;
                response.ContentType = "application/json";
                var msg = string.Empty;
                switch (response.StatusCode)
                {
                    case 400:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        msg = "Badrequest";
                        break;
                    case 401:
                        // Unauthorized error
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        msg = "Unauthorized";
                        break;
                    case 403:
                        // Unauthorized error
                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                        msg = "Unauthorized";
                        break;
                    case 404:
                        // not found error
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        msg = "NotFound";
                        break;
                  
                }

                var result = new ErrorDetails()
                {
                    StatusCode = response.StatusCode,
                    Message = msg,
                }.ToString();
                await response.WriteAsync(result);
            }
            catch (Exception e)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                var result = new ErrorDetails()
                {
                    StatusCode = response.StatusCode,
                    Message = e.Message,
                }.ToString();
                await response.WriteAsync(result);
            }
        }
    }
}
