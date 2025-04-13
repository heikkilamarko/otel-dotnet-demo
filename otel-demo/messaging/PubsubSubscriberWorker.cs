using System.Diagnostics;
using Google.Api.Gax;
using Google.Cloud.PubSub.V1;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace Messaging;

public class PubsubSubscriberWorker<TMessageHandler>(
    PubsubSubscriberOptions<TMessageHandler> options,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<PubsubSubscriberWorker<TMessageHandler>> logger) : BackgroundService where TMessageHandler : class, IPubsubMessageHandler
{
    private SubscriberClient _client;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("start subscriber worker: {project}, {topic}, {subscription}", options.Project, options.Topic, options.Subscription);

        if (options.UseEmulator) logger.LogInformation("using pubsub emulator");

        if (options.UseEmulator && options.CreateSubscription)
        {
            logger.LogInformation("create subscription");
            await CreateSubscriptionAsync(stoppingToken);
        }

        while (true)
        {
            try
            {
                logger.LogInformation("start subscriber");

                await using (_client = await CreateSubscriberClientAsync(stoppingToken))
                {
                    await _client.StartAsync(async (message, token) =>
                    {
                        var parentContext = Propagators.DefaultTextMapPropagator.Extract(
                            default,
                            message.Attributes,
                            (attrs, key) => attrs.TryGetValue(key, out var value) ? [value] : []);

                        Baggage.Current = parentContext.Baggage;

                        using var activity = MessagingActivitySource.PubsubActivitySource.StartActivity("consume-message", ActivityKind.Consumer, parentContext.ActivityContext);

                        if (activity != null)
                        {
                            activity.SetTag("messaging.system", "gcp.pubsub");
                            activity.SetTag("messaging.subscription", _client.SubscriptionName);
                        }

                        using var scope = serviceScopeFactory.CreateScope();
                        var handler = scope.ServiceProvider.GetRequiredService<TMessageHandler>();
                        return await handler.HandleAsync(message, token);
                    });
                }
            }
            catch (Exception err)
            {
                logger.LogError(err, "subscriber error");
            }
            finally
            {
                _client = null;
                logger.LogInformation("subscriber stopped");
            }

            if (stoppingToken.IsCancellationRequested) break;

            await Task.Delay(TimeSpan.FromSeconds(options.SubscriptionIntervalSeconds), stoppingToken);
        }

        logger.LogInformation("exit subscriber worker");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _client?.StopAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }

    private Task<SubscriberClient> CreateSubscriberClientAsync(CancellationToken cancellationToken)
    {
        if (options.UseEmulator)
        {
            return new SubscriberClientBuilder
            {
                EmulatorDetection = EmulatorDetection.EmulatorOnly,
                SubscriptionName = SubscriptionName.FromProjectSubscription(options.Project, options.Subscription),
                Settings = new SubscriberClient.Settings
                {
                    FlowControlSettings = new FlowControlSettings(options.MaxOutstandingElements, options.MaxOutstandingByteCount)
                }
            }.BuildAsync(cancellationToken);
        }

        return new SubscriberClientBuilder
        {
            SubscriptionName = SubscriptionName.FromProjectSubscription(options.Project, options.Subscription),
            Settings = new SubscriberClient.Settings
            {
                AckExtensionWindow = TimeSpan.FromSeconds(options.AckExtensionWindowSeconds),
                AckDeadline = TimeSpan.FromSeconds(options.AckDeadlineSeconds),
                FlowControlSettings = new FlowControlSettings(options.MaxOutstandingElements, options.MaxOutstandingByteCount)
            }
        }.BuildAsync(cancellationToken);
    }

    private async Task CreateSubscriptionAsync(CancellationToken cancellationToken)
    {
        var apiClient = await new SubscriberServiceApiClientBuilder
        {
            EmulatorDetection = EmulatorDetection.EmulatorOnly
        }.BuildAsync(cancellationToken);

        if (options.DeadLetterTopic != null)
        {
            await CreateTopicAsync(options.Project, options.DeadLetterTopic, cancellationToken);
        }

        try
        {
            var request = new Subscription
            {
                TopicAsTopicName = new TopicName(options.Project, options.Topic),
                SubscriptionName = new SubscriptionName(options.Project, options.Subscription),
                AckDeadlineSeconds = options.AckDeadlineSeconds,
                EnableMessageOrdering = options.EnableMessageOrdering,
                DeadLetterPolicy = options.DeadLetterTopic != null ? new DeadLetterPolicy
                {
                    DeadLetterTopic = new TopicName(options.Project, options.DeadLetterTopic).ToString(),
                    MaxDeliveryAttempts = options.MaxDeliveryAttempts
                } : null
            };

            await apiClient.CreateSubscriptionAsync(request, cancellationToken);
        }
        catch (RpcException e) when (e.Status.StatusCode == StatusCode.AlreadyExists)
        {
            logger.LogInformation("subscription already exists");
        }
    }

    private async Task CreateTopicAsync(string project, string topic, CancellationToken cancellationToken)
    {
        var apiClient = await new PublisherServiceApiClientBuilder
        {
            EmulatorDetection = EmulatorDetection.EmulatorOnly
        }.BuildAsync(cancellationToken);

        try
        {
            var request = new Topic
            {
                TopicName = new TopicName(project, topic)
            };

            await apiClient.CreateTopicAsync(request, cancellationToken);
        }
        catch (RpcException e) when (e.Status.StatusCode == StatusCode.AlreadyExists)
        {
            logger.LogInformation("topic already exists");
        }
    }
}
