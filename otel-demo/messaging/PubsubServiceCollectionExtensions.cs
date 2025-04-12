using Microsoft.Extensions.DependencyInjection;

namespace Messaging;

public static class PubsubServiceCollectionExtensions
{
    public static void AddPubsubSubscriber<TMessageHandler>(this IServiceCollection services, PubsubSubscriberOptions<TMessageHandler> options)
        where TMessageHandler : class, IPubsubMessageHandler
    {
        services.AddSingleton(options);
        services.AddTransient<TMessageHandler>();
        services.AddHostedService<PubsubSubscriberWorker<TMessageHandler>>();
    }

    public static void AddPubsubPublisher<T>(this IServiceCollection services, PubsubPublisherOptions<T> options)
    {
        services.AddSingleton(options);
        services.AddSingleton<PubsubPublisher<T>>();
    }
}
