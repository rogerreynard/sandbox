using Newtonsoft.Json;

namespace Foundation.ImageMetadataGenerator.Models
{
    [JsonObject]
    public class VisionResponseChoice

    {
        [JsonProperty("message")]
        public VisionResponseMessage Message { get; set; }
    }
}