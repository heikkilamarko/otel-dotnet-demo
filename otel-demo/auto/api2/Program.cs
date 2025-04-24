using Messaging;
using OtelDemo.Api2.Extensions;
using OtelDemo.Api2.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<AppActivitySource>();

builder.Services.AddScoped<PostRollDiceHandler>();

builder.Services.AddPubsubPublisher<DemoMessage>(new(
    Project: "local",
    Topic: "demo_topic",
    UseEmulator: true,
    CreateTopic: true));

var app = builder.Build();

app.MapPost("/rolldice/{player}", async (PostRollDiceHandler handler, string player) =>
{
    var req = new PostRollDiceRequest { Player = player };
    var res = await handler.HandleAsync(req);
    return Results.Ok(res);
});

await app.RunAsync();
