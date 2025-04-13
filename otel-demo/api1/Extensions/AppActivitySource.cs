using System.Diagnostics;

namespace OtelDemo.Api1.Extensions;

public class AppActivitySource(IConfiguration configuration)
{
    public ActivitySource ActivitySource { get; } = new(configuration.GetAppName(), configuration.GetAppVersion());
}
