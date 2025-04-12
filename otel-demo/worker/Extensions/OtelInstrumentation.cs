using System.Diagnostics;

namespace OtelDemo.Worker.Extensions;

public class OtelInstrumentation(IConfiguration configuration)
{
    public ActivitySource ActivitySource { get; } = new(configuration.GetAppName(), configuration.GetAppVersion());
}
