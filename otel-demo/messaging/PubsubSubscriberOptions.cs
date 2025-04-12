namespace Messaging;

public record PubsubSubscriberOptions<T>(
    string Project,
    string Topic,
    string Subscription,
    string DeadLetterTopic = null,
    int MaxDeliveryAttempts = 5,
    int AckExtensionWindowSeconds = 4,
    int AckDeadlineSeconds = 10,
    int SubscriptionIntervalSeconds = 10,
    long MaxOutstandingElements = 5,
    long MaxOutstandingByteCount = 10240,
    bool EnableMessageOrdering = false,
    bool UseEmulator = false,
    bool CreateSubscription = false
);
