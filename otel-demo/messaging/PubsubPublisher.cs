using Google.Api.Gax;
using Google.Cloud.PubSub.V1;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Messaging;

public sealed class PubsubPublisher<T> : IAsyncDisposable
{
    private readonly PubsubPublisherOptions<T> _options;
    private readonly ILogger<PubsubPublisher<T>> _logger;

    private readonly Task _initializeTask;

    private PublisherClient _client;

    public PubsubPublisher(PubsubPublisherOptions<T> options, ILogger<PubsubPublisher<T>> logger)
    {
        _options = options;
        _logger = logger;
        _initializeTask = InitializeAsync();
    }

    public PubsubPublisherOptions<T> Options => _options;

    public async ValueTask DisposeAsync()
    {
        if (_client != null) await _client.DisposeAsync();
    }

    public async Task PublishAsync(PubsubMessage message)
    {
        await _initializeTask;
        await _client.PublishAsync(message);
    }

    private async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("initialize publisher: {project}, {topic}", _options.Project, _options.Topic);

        if (_options.UseEmulator) _logger.LogInformation("using pubsub emulator");

        if (_options.UseEmulator && _options.CreateTopic)
        {
            _logger.LogInformation("create topic");
            await CreateTopicAsync(cancellationToken);
        }

        _client = await CreatePublisherClientAsync(cancellationToken);
    }

    private Task<PublisherClient> CreatePublisherClientAsync(CancellationToken cancellationToken = default)
    {
        if (_options.UseEmulator)
        {
            return new PublisherClientBuilder
            {
                EmulatorDetection = EmulatorDetection.EmulatorOnly,
                TopicName = new TopicName(_options.Project, _options.Topic)

            }.BuildAsync(cancellationToken);
        }

        return new PublisherClientBuilder
        {
            TopicName = new TopicName(_options.Project, _options.Topic)
        }.BuildAsync(cancellationToken);
    }

    private async Task CreateTopicAsync(CancellationToken cancellationToken)
    {
        var apiClient = await new PublisherServiceApiClientBuilder
        {
            EmulatorDetection = EmulatorDetection.EmulatorOnly
        }.BuildAsync(cancellationToken);

        try
        {
            var request = new Topic
            {
                TopicName = new TopicName(_options.Project, _options.Topic)
            };

            await apiClient.CreateTopicAsync(request, cancellationToken);
        }
        catch (RpcException e) when (e.Status.StatusCode == StatusCode.AlreadyExists)
        {
            _logger.LogInformation("topic already exists");
        }
    }
}
