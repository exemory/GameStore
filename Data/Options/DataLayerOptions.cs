namespace Data.Options;

public class DataLayerOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public bool EnableSensitiveDataLogging { get; set; }
}