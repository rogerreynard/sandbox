using Newtonsoft.Json;

namespace Foundation.ImageMetadataGenerator.Models
{
    [JsonObject]
    public class VisionResponse
    {
        [JsonProperty("choices")]
        public VisionResponseChoice[] Choices { get; set; }
    }
}