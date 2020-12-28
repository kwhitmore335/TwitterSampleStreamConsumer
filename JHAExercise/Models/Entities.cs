using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace JHAExercise.Models
{
    [JsonObject()]
    public class Entities
    {
        [JsonProperty("urls")]
        public List<EntityUrl> Urls { get; set; }

        [JsonProperty("hashtags")]
        public  List<Hashtag> Hashtags { get; set; }

        public IEnumerable<string> Emojis { get; set; }
    }
}
