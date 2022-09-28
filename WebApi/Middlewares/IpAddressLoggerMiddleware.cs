namespace WebApi.Middlewares;

/// <summary>
/// Middleware for logging IP addresses of requests
/// </summary>
public class IpAddressLoggerMiddleware
{
    private readonly ILogger<IpAddressLoggerMiddleware> _logger;
    private readonly RequestDelegate _next;

    /// <summary>
    /// Constructor for initializing a <see cref="IpAddressLoggerMiddleware"/> class instance
    /// </summary>
    /// <param name="logger">Logger</param>
    /// <param name="next">Next middleware in request pipeline</param>
    public IpAddressLoggerMiddleware(ILogger<IpAddressLoggerMiddleware> logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    /// <summary>
    /// Middleware logic for logging ip address of remote client and request path
    /// </summary>
    /// <param name="context">Context of the current http request</param>
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Connection.RemoteIpAddress != null)
        {
            LogRequestIpAddress(context);
        }

        await _next(context);
    }

    /// <summary>
    /// Log the ip address of the remote client and the path of the request
    /// </summary>
    /// <param name="context">Context of the current http request</param>
    private void LogRequestIpAddress(HttpContext context)
    {
        var requestPath = $"{context.Request.Path}{context.Request.QueryString}";
        var ipAddress = context.Connection.RemoteIpAddress;

        _logger.LogInformation("Connection from {IpAddress}. Request path: {RequestPath}", ipAddress, requestPath);
    }
}