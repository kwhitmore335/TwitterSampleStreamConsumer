using Newtonsoft.Json;
using System;

namespace JHAExercise.Models
{
    [JsonObject()]
    public class Tweet
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset Created { get; set; }

        [JsonProperty("entities")]
        public Entities Entities { get; set; }        
    }
}
