using System.Diagnostics;

namespace OtelDemo.Api2.Extensions;

public class AppActivitySource
{
    public ActivitySource ActivitySource { get; } = new("OtelDemo.Api2");
}
