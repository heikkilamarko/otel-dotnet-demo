using System.Text.Json.Serialization;
using OtelDemo.Api2.Extensions;

namespace OtelDemo.Api2.Handlers;

public class GetRollDiceRequest
{
    [JsonPropertyName("player")]
    public string Player { get; set; }
}

public class GetRollDiceResult
{
    [JsonPropertyName("results")]
    public List<int> Results { get; set; }
}

public class GetRollDiceHandler(OtelInstrumentation instrumentation, ILogger<GetRollDiceHandler> logger)
{
    public async Task<GetRollDiceResult> HandleAsync(GetRollDiceRequest request)
    {
        logger.LogInformation("{player} is rolling the dice", request.Player);

        var results = new List<int>();

        for (var i = 0; i < 3; i++)
        {
            results.Add(await RollDiceAsync());
        }

        return new GetRollDiceResult { Results = results };
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
