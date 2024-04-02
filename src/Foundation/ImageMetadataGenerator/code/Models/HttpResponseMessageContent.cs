using Newtonsoft.Json;

namespace Foundation.ImageMetadataGenerator.Models
{
    [JsonObject]
    public class HttpResponseMessageContent
    {
        public string AltText { get; set; }
        public string Description { get; set; }
    }
}