using System.Net;
using System.Text.Json;
using SchoolOfDevs.Exceptions;

namespace SchoolOfDevs.Middleware
{
    public class ErroHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErroHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = error switch
                {
                    BadRequestException => (int)HttpStatusCode.BadRequest,
                    KeyNotFoundException => (int)HttpStatusCode.NotFound,
                    ForbiddenException => (int)HttpStatusCode.Forbidden,
                    _ => (int)HttpStatusCode.InternalServerError
                };

                var result = JsonSerializer.Serialize(new { message = error?.Message });
                await response.WriteAsync(result);
            }
        }
    }
}