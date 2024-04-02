using Newtonsoft.Json;

namespace Foundation.ImageMetadataGenerator.Models
{
    [JsonObject]
    public class VisionRequestImageUrlType
    {
        [JsonProperty("type")]
        public string Type => "image_url";

        [JsonProperty("image_url")]
        public VisionRequestImageUrlProperty ImageUrl { get; set; }
    }
}