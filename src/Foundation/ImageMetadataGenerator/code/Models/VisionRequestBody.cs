using System.Collections;
using Newtonsoft.Json;

namespace Foundation.ImageMetadataGenerator.Models
{
    [JsonObject]
    public class VisionRequestBody
    {
        [JsonProperty("model")]
        public string Model { get; set; } = "gpt-4-vision-preview";

        [JsonProperty("messages")]
        public ArrayList Messages { get; set; }

        [JsonProperty("max_tokens")]
        public int MaxTokens { get; set; } = 300;
    }
}