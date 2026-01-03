using System.Text.Json;
using System.Text.Json.Serialization;

public class VoteType
{
    public enum Type
    {
        upvote = 1,
        downvote = 0
    }
}

public class VoteTypeConverter : JsonConverter<VoteType.Type>
{
    public override VoteType.Type Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();
        
        if (string.Equals(stringValue, "upvote", StringComparison.OrdinalIgnoreCase))
            return VoteType.Type.upvote;
        if (string.Equals(stringValue, "downvote", StringComparison.OrdinalIgnoreCase))
            return VoteType.Type.downvote;
        
        throw new JsonException($"Unknown vote type: {stringValue}");
    }

    public override void Write(Utf8JsonWriter writer, VoteType.Type value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

public class VoteRequest
{
    [JsonPropertyName("vote_type")]
    [JsonConverter(typeof(VoteTypeConverter))]
    public VoteType.Type VoteType { get; set; }
}