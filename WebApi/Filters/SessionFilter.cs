using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using ISession = Business.Interfaces.ISession;

namespace WebApi.Filters
{
    /// <summary>
    /// Action filter for session initialization
    /// </summary>
    public class SessionFilter : IAsyncActionFilter
    {
        private readonly ISession _session;

        /// <summary>
        /// Constructor for initializing a <see cref="SessionFilter"/> class instance
        /// </summary>
        /// <param name="session">Current session</param>
        public SessionFilter(ISession session)
        {
            _session = session;
        }

        /// <summary>
        /// Session filter logic
        /// </summary>
        /// <param name="context">Context of the current http request</param>
        /// <param name="next">Next action filter in pipeline</param>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var claims = context.HttpContext.User;

            if (claims.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            {
                var userId = new Guid(claims.FindFirstValue(ClaimTypes.NameIdentifier));
                var userRoles = claims.FindAll(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);

                _session.Initialize(userId, userRoles);
            }

            await next();
        }
    }
}