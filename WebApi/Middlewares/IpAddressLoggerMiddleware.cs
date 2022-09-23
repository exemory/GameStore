using Microsoft.Extensions.Options;
using WebApi.Options;

namespace WebApi.Middlewares;

/// <summary>
/// Middleware for logging IP addresses of requests
/// </summary>
public class IpAddressLoggerMiddleware
{
    private readonly IpAddressLoggerOptions _options;
    private readonly RequestDelegate _next;

    /// <summary>
    /// Constructor for initializing a <see cref="IpAddressLoggerMiddleware"/> class instance
    /// </summary>
    /// <param name="options">IP logger options</param>
    /// <param name="next">Next middleware in request pipeline</param>
    public IpAddressLoggerMiddleware(IOptions<IpAddressLoggerOptions> options, RequestDelegate next)
    {
        _options = options.Value;
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
            await LogRequestIpAddress(context);
        }

        await _next(context);
    }

    /// <summary>
    /// Log the ip address of the remote client and the path of the request
    /// </summary>
    /// <param name="context">Context of the current http request</param>
    private async Task LogRequestIpAddress(HttpContext context)
    {
        await using var writer = new StreamWriter(_options.FullFilePath, true);

        var requestPath = $"{context.Request.Path}{context.Request.QueryString}";
        var ipAddress = context.Connection.RemoteIpAddress;
        
        await writer.WriteLineAsync($"{ipAddress} {requestPath}");
    }
}