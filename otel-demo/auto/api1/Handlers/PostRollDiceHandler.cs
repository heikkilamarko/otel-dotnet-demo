using System.Text.Json.Serialization;

namespace OtelDemo.Api1.Handlers;

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

public class PostRollDiceHandler(IHttpClientFactory httpClientFactory, ILogger<PostRollDiceHandler> logger)
{
    public async Task<PostRollDiceResult> HandleAsync(PostRollDiceRequest request)
    {
        logger.LogInformation("rolling dice for {player}", request.Player);

        using var client = httpClientFactory.CreateClient("otel-demo-api2");

        var response = await client.PostAsync($"rolldice/{request.Player}", null);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("failed to roll dice for {player}: {statusCode}", request.Player, response.StatusCode);
            response.EnsureSuccessStatusCode();
        }

        var result = await response.Content.ReadFromJsonAsync<PostRollDiceResult>();

        logger.LogInformation("rolled dice for {player}", request.Player);

        return result;
    }
}
