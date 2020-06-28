using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TangentWatson.Entities
{
    public class WatsonResponse
    {
        [JsonPropertyName("document_tone")]
        public DocumentTone DocumentTone { get; set; }
        [JsonPropertyName("sentences_tone")]
        public SentencesTone[] SentencesTone { get; set; }
    }
}
