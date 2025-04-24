using System.Diagnostics;

namespace OtelDemo.Worker.Extensions;

public class AppActivitySource
{
    public ActivitySource ActivitySource { get; } = new("OtelDemo.Worker");
}
