using System.Diagnostics;

namespace WebApi.Middlewares;

/// <summary>
/// Middleware for logging execution performance
/// </summary>
public class PerformanceLoggerMiddleware
{
    private readonly ILogger<PerformanceLoggerMiddleware> _logger;
    private readonly RequestDelegate _next;

    /// <summary>
    /// Constructor for initializing a <see cref="PerformanceLoggerMiddleware"/> class instance
    /// </summary>
    /// <param name="logger">Logger</param>
    /// <param name="next">Next middleware in request pipeline</param>
    public PerformanceLoggerMiddleware(ILogger<PerformanceLoggerMiddleware> logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    /// <summary>
    /// Middleware logic for measuring and logging execution performance
    /// </summary>
    /// <param name="context">Context of the current http request</param>
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        await _next(context);

        stopwatch.Stop();
        LogExecutionTime(stopwatch.Elapsed);
    }

    /// <summary>
    /// Log execution time
    /// </summary>
    /// <param name="time">Execution time</param>
    private void LogExecutionTime(TimeSpan time)
    {
        _logger.LogInformation("Request executed in {Time}", time);
    }
}