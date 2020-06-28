using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TangentWatson.Entities
{
    public class DocumentTone
    {
        [JsonPropertyName("tones")]
        public Tone[] Tones { get; set; }
    }
}
