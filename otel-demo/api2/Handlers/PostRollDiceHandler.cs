using System.Text.Json.Serialization;
using Messaging;
using OtelDemo.Api2.Extensions;

namespace OtelDemo.Api2.Handlers;

public class PostRollDiceRequest
{
    [JsonPropertyName("player")]
    public string Player { get; set; }
}

public class PostRollDiceResult
{
    [JsonPropertyName("results")]
    public List<int> Results { get; set; }
}

public class DemoMessage
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonPropertyName("name")]
    public string Name { get; set; } = "demo message";
}

public class PostRollDiceHandler(PubsubPublisher<DemoMessage> publisher, OtelInstrumentation instrumentation, ILogger<PostRollDiceHandler> logger)
{
    public async Task<PostRollDiceResult> HandleAsync(PostRollDiceRequest request)
    {
        logger.LogInformation("{player} is rolling the dice", request.Player);

        var results = new List<int>();

        for (var i = 0; i < 3; i++)
        {
            results.Add(await RollDiceAsync());
        }

        var message = new DemoMessage
        {
            Id = Guid.NewGuid(),
            Name = $"{request.Player} rolled {string.Join(", ", results)}"
        }.ToJsonPubsubMessage();

        using (var activity = instrumentation.ActivitySource.StartPublishActivity($"{publisher.Options.Project}/{publisher.Options.Topic}", message))
        {
            await publisher.PublishAsync(message);
        }

        return new PostRollDiceResult { Results = results };
    }

    private async Task<int> RollDiceAsync()
    {
        using var activity = instrumentation.ActivitySource.StartActivity("roll-dice");
        await Task.Delay(2); // Simulate some work
        var result = Random.Shared.Next(1, 7);
        activity?.SetTag("dice.result", result);
        return result;
    }
}
