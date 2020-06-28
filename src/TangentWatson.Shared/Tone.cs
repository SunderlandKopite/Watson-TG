using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TangentWatson.Entities
{
    public class Tone
    {
        [JsonPropertyName("score")]
        public decimal Score { get; set; }
        [JsonPropertyName("tone_name")]
        public string Name { get; set; }
    }
}
