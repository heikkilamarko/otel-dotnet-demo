using System.Text.Json.Serialization;

namespace OtelDemo.Api1.Handlers;

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

public class GetRollDiceHandler(IHttpClientFactory httpClientFactory, ILogger<GetRollDiceHandler> logger)
{
    public async Task<GetRollDiceResult> HandleAsync(GetRollDiceRequest request)
    {
        logger.LogInformation("rolling dice for {player}", request.Player);

        using var client = httpClientFactory.CreateClient("otel-demo-api2");

        var response = await client.GetAsync($"rolldice/{request.Player}");

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("failed to roll dice for {player}: {statusCode}", request.Player, response.StatusCode);
            response.EnsureSuccessStatusCode();
        }

        var result = await response.Content.ReadFromJsonAsync<GetRollDiceResult>();

        logger.LogInformation("rolled dice for {player}", request.Player);

        return result;
    }
}
