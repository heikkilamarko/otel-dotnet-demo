using Messaging;
using OtelDemo.Worker.Extensions;
using OtelDemo.Worker.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.AddOtel();

builder.Services.AddPubsubSubscriber<DemoMessageHandler>(new(
    Project: "local",
    Topic: "demo_topic",
    Subscription: "demo_subscription",
    UseEmulator: true,
    CreateSubscription: true));

var app = builder.Build();

await app.RunAsync();
