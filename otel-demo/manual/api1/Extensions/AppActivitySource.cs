using System.Diagnostics;

namespace OtelDemo.Api1.Extensions;

public class AppActivitySource
{
    public ActivitySource ActivitySource { get; } = new("OtelDemo.Api1");
}
