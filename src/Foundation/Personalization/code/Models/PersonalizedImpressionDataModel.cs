using System;
using Newtonsoft.Json;

namespace Foundation.Personalization.Models
{
    [Serializable]
    public class PersonalizedImpressionDataModel
    {
        [JsonProperty("component_name")]
        public string ComponentName { get; set; }
        [JsonProperty("condition_name")]
        public string ConditionName { get; set; }
        [JsonProperty("condition_order")]
        public string ConditionOrder { get; set; }
        [JsonProperty("condition_state")]
        public string ConditionState { get; set; }
        [JsonProperty("personalized_rendering")]
        public string PersonalizedRendering { get; set; }
        [JsonProperty("personalized_content")]
        public string PersonalizedContent { get; set; }
        [JsonProperty("personalized_content_id")]
        public string PersonalizedContentId { get; set; }
    }
}