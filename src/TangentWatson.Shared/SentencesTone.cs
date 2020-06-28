using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TangentWatson.Entities
{
    public class SentencesTone
    {
        [JsonPropertyName("sentence_id")]
        int Sentence { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("tones")]
        public Tone[] Tones { get; set; }
    }
}
