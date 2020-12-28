using System;
using Newtonsoft.Json;

namespace JHAExercise.Models
{
    [JsonObject()]
    public class EntityUrl
    {
        [JsonProperty("expanded_url")]
        public string Url { get; set; }
    }
}
