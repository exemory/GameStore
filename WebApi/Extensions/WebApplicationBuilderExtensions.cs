using WebApi.Options;

namespace WebApi.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddOptions(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var config = builder.Configuration;

        services.Configure<IpAddressLoggerOptions>(config.GetSection(nameof(IpAddressLoggerOptions)));
        
        return builder;
    }
}