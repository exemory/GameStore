namespace WebApi.Options;

public class IpAddressLoggerOptions
{
    public string FilePath { get; set; } = default!;
    public string FullFilePath => Path.Combine(AppContext.BaseDirectory, FilePath);
}