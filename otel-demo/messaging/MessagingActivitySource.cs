using System.Diagnostics;

namespace Messaging;

internal static class MessagingActivitySource
{
    internal static readonly string PubsubActivitySourceName = "Messaging.Pubsub";
    internal static readonly ActivitySource PubsubActivitySource = new(PubsubActivitySourceName, "1.0.0");
}
