using System.Text.Json.Serialization;

namespace TangentWatson.Entities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ResponseStatus
    {
        Pending,
        Complete,
        Failed
    }
}