using System;
using Newtonsoft.Json;

namespace Foundation.PageExtender.Models
{
    [Serializable]
    public class PersonalizedImpressionDataModel
    {
        [JsonProperty("rendering_name")]
        public string RenderingName { get; set; }
        [JsonProperty("rendering_path")]
        public string RenderingPath { get; set; }
        [JsonProperty("rendering_id")]
        public string RenderingID { get; set; }
        [JsonProperty("rule_name")]
        public string RuleName { get; set; }
        [JsonProperty("rendering_state")]
        public string RenderingState { get; set; }
        [JsonProperty("datasource_path")]
        public string DatasourcePath { get; set; }
        [JsonProperty("datasource_id")]
        public string DatasourceID { get; set; }
    }
}