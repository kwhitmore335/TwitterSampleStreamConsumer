using System;
using Newtonsoft.Json;

namespace JHAExercise.Models
{
    [JsonObject()]
    public class Hashtag
    {
        [JsonProperty("tag")]
        public string Tag { get; set; }
    }
}
