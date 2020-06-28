using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TangentWatson.Entities
{
    /// <summary>
    /// The message with associated data
    /// </summary>
    public class RatedComment
    {
        [BsonId()]
        public Guid UniqueID { get; set; } = Guid.NewGuid();

        public string Message { get; set; }

        public ResponseStatus Status { get; set; } = ResponseStatus.Pending;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public WatsonResponse WatsonResponse { get; set; }
    }
}