using Messaging;
using Microsoft.EntityFrameworkCore;
using OtelDemo.Worker.Data;
using OtelDemo.Worker.Extensions;
using OtelDemo.Worker.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<AppActivitySource>();

builder.Services.AddDbContext<DemoContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Demo")));

builder.Services.AddPubsubSubscriber<DemoMessageHandler>(new(
    Project: "local",
    Topic: "demo_topic",
    Subscription: "demo_subscription",
    UseEmulator: true,
    CreateSubscription: true));

var app = builder.Build();

await app.RunAsync();
