using System.Diagnostics;
using Google.Cloud.PubSub.V1;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace Messaging;

public static class ActivitySourceExtensions
{
    public static Activity StartPublishActivity(this ActivitySource activitySource, string topicId, PubsubMessage message)
    {
        var activity = activitySource.StartActivity("publish-message", ActivityKind.Producer);

        activity?.SetTag("messaging.system", "gcp.pubsub");
        activity?.SetTag("messaging.topic", topicId);

        Propagators.DefaultTextMapPropagator.Inject(
            new PropagationContext(activity.Context, Baggage.Current),
            message.Attributes,
            (dict, key, value) => dict[key] = value);

        return activity;
    }

    public static Activity StartConsumeActivity(this ActivitySource activitySource, string subscriptionId, PubsubMessage message)
    {
        var parentContext = Propagators.DefaultTextMapPropagator.Extract(
            default,
            message.Attributes,
            (attrs, key) => attrs.TryGetValue(key, out var value) ? [value] : []);

        Baggage.Current = parentContext.Baggage;

        var activity = activitySource.StartActivity("consume-message", ActivityKind.Consumer, parentContext.ActivityContext);

        activity?.SetTag("messaging.system", "gcp.pubsub");
        activity?.SetTag("messaging.subscription", subscriptionId);

        return activity;
    }
}
