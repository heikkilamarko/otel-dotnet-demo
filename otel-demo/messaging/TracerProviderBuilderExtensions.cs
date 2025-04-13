using OpenTelemetry.Trace;

namespace Messaging;

public static class TracerProviderBuilderExtensions
{
    public static TracerProviderBuilder AddMessagingInstrumentation(this TracerProviderBuilder builder)
    {
        return builder.AddSource(MessagingActivitySource.PubsubActivitySourceName);
    }
}
