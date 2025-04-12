using System.Text.Json.Serialization;
using Google.Cloud.PubSub.V1;
using Messaging;
using OtelDemo.Worker.Extensions;

namespace OtelDemo.Worker.Handlers;

public class DemoMessage
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public class DemoMessageHandler(OtelInstrumentation instrumentation, ILogger<DemoMessageHandler> logger) : IPubsubMessageHandler
{
    public async Task<SubscriberClient.Reply> HandleAsync(PubsubMessage message, CancellationToken cancellationToken)
    {
        using var activity = instrumentation.ActivitySource.StartConsumeActivity("demo_subscription", message);

        await Task.Delay(10, cancellationToken); // Simulate some work

        DemoMessage messageData;
        try
        {
            messageData = message.JsonDeserialize<DemoMessage>();
        }
        catch (Exception err)
        {
            logger.LogWarning(err, "[{message_id}] invalid message", message.MessageId);
            return SubscriberClient.Reply.Nack;
        }

        logger.LogInformation("[{message_id}] {id} {name}", message.MessageId, messageData.Id, messageData.Name);
        return SubscriberClient.Reply.Ack;
    }
}
