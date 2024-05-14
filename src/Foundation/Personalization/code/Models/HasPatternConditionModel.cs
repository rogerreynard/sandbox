using Newtonsoft.Json;

namespace Foundation.Personalization.Models
{
    public class HasPatternConditionModel
    {
        [JsonProperty("pattern_name")]
        public string PatternName { get; set; }

        [JsonProperty("profile_name")]
        public string ProfileName { get; set; }
    }
}