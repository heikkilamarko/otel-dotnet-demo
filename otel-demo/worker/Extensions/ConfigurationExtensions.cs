namespace OtelDemo.Worker.Extensions;

public static class ConfigurationExtensions
{
    public static string GetAppName(this IConfiguration configuration)
    {
        return configuration["APP_NAME"];
    }

    public static string GetAppVersion(this IConfiguration configuration)
    {
        return configuration["APP_VERSION"];
    }

    public static Uri GetOtelOtlpEndpoint(this IConfiguration configuration)
    {
        return new Uri(configuration["OTEL_OTLP_ENDPOINT"]);
    }
}
