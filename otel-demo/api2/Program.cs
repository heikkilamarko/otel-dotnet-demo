using OtelDemo.Api2.Extensions;
using OtelDemo.Api2.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.AddOtel();

builder.Services.AddScoped<GetRollDiceHandler>();

var app = builder.Build();

app.MapGet("/rolldice/{player}", async (GetRollDiceHandler handler, string player) =>
{
    var req = new GetRollDiceRequest { Player = player };
    var res = await handler.HandleAsync(req);
    return Results.Ok(res);
});

await app.RunAsync();
