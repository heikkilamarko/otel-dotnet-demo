using System.Text.Json.Serialization;
using Google.Cloud.PubSub.V1;
using Messaging;
using OtelDemo.Worker.Data;

namespace OtelDemo.Worker.Handlers;

public class DemoMessage
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public class DemoMessageHandler(DemoContext db, ILogger<DemoMessageHandler> logger) : IPubsubMessageHandler
{
    public async Task<SubscriberClient.Reply> HandleAsync(PubsubMessage message, CancellationToken cancellationToken)
    {
        DemoMessage messageData;
        try
        {
            messageData = message.JsonDeserialize<DemoMessage>();

            db.messages.Add(new()
            {
                id = messageData.Id,
                name = messageData.Name,
                created_at = DateTime.UtcNow,
            });
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception err)
        {
            logger.LogWarning(err, "[{message_id}] invalid message", message.MessageId);
            return SubscriberClient.Reply.Ack;
        }

        logger.LogInformation("[{message_id}] {id} {name}", message.MessageId, messageData.Id, messageData.Name);
        return SubscriberClient.Reply.Ack;
    }
}
