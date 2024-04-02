using Newtonsoft.Json;

namespace Foundation.ImageMetadataGenerator.Models
{
    [JsonObject]
    public class VisionResponseMessage
    {
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}