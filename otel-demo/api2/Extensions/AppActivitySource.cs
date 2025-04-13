using System.Diagnostics;

namespace OtelDemo.Api2.Extensions;

public class AppActivitySource(IConfiguration configuration)
{
    public ActivitySource ActivitySource { get; } = new(configuration.GetAppName(), configuration.GetAppVersion());
}
