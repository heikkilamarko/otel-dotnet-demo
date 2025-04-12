using System.Text.Json;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;

namespace Messaging;

public static class PubsubMessageExtensions
{
    private static readonly JsonSerializerOptions jsonSerializerOptions = new(JsonSerializerDefaults.Web);

    public static T JsonDeserialize<T>(this PubsubMessage message)
    {
        return JsonSerializer.Deserialize<T>(message.Data.ToStringUtf8(), jsonSerializerOptions);
    }

    public static bool TryJsonDeserialize<T>(this PubsubMessage message, out T result)
    {
        try
        {
            result = message.JsonDeserialize<T>();
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }

    public static PubsubMessage ToJsonPubsubMessage(this object data)
    {
        return new PubsubMessage
        {
            Data = ByteString.CopyFromUtf8(JsonSerializer.Serialize(data, jsonSerializerOptions))
        };
    }
}
