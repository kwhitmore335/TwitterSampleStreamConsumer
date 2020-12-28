using Newtonsoft.Json;
using System;

namespace JHAExercise.Models
{
    [JsonObject()]
    public class EmojiData
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("unified")]
        public string UnicodeCodepoint { get; set; }      
    }
}
