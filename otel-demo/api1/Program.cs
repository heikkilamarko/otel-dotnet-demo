using OtelDemo.Api1.Extensions;
using OtelDemo.Api1.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.AddOtel();

builder.Services.AddScoped<PostRollDiceHandler>();

builder.Services.AddHttpClient("otel-demo-api2", client =>
{
    client.BaseAddress = builder.Configuration.GetApi2BaseAddress();
});

var app = builder.Build();

app.MapPost("/rolldice/{player}", async (PostRollDiceHandler handler, string player) =>
{
    var req = new PostRollDiceRequest { Player = player };
    var res = await handler.HandleAsync(req);
    return Results.Ok(res);
});

await app.RunAsync();
