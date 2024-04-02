using System.Collections;
using Newtonsoft.Json;

namespace Foundation.ImageMetadataGenerator.Models
{
    [JsonObject]
    public class VisionRequestUserRole
    {
        [JsonProperty("role")]
        public string Role => "user";

        [JsonProperty("content")]
        public ArrayList Content { get; set; }
    }
}