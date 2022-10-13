using Business.Exceptions;

namespace WebApi.Middlewares
{
    /// <summary>
    /// Middleware for handling application exceptions and converting them to responses
    /// </summary>
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Constructor for initializing a <see cref="ErrorHandlerMiddleware"/> class instance
        /// </summary>
        /// <param name="next">Next middleware in request pipeline</param>
        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Middleware logic for handling exceptions and convert them to the error response
        /// </summary>
        /// <param name="context">Context of the current http request</param>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (GameStoreException exception)
            {
                context.Response.StatusCode = exception switch
                {
                    RegistrationException e => StatusCodes.Status400BadRequest,
                    AuthenticationException e => StatusCodes.Status401Unauthorized,
                    AccessDeniedException e => StatusCodes.Status403Forbidden,
                    NotFoundException e => StatusCodes.Status404NotFound,
                    _ => StatusCodes.Status400BadRequest
                };

                context.Response.ContentType = "application/json";

                var response = new
                {
                    status = context.Response.StatusCode,
                    message = exception.Message
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}