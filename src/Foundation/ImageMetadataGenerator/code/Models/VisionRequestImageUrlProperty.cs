using Newtonsoft.Json;

namespace Foundation.ImageMetadataGenerator.Models
{
    [JsonObject]
    public class VisionRequestImageUrlProperty
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("detail")]
        public string Detail => "low";
    }
}