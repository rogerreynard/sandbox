using Newtonsoft.Json;

namespace Foundation.ImageMetadataGenerator.Models
{
    [JsonObject]
    public class VisionRequestTextType
    {
        [JsonProperty("type")] public string Type => "text";

        [JsonProperty("text")] public string Text => "Please provide alt text and a description for the image provided.";
    }
}