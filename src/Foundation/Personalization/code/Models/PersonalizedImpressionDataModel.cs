using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Foundation.Personalization.Models
{
    [Serializable]
    public class PersonalizedImpressionDataModel
    {
        [JsonProperty("component_name")]
        public string ComponentName { get; set; }
        [JsonProperty("rendering_path")]
        public string RenderingPath { get; set; }
        [JsonProperty("rendering_id")]
        public string RenderingID { get; set; }
        [JsonProperty("datasource_path")]
        public string DatasourcePath { get; set; }
        [JsonProperty("datasource_id")]
        public string DatasourceID { get; set; }
        [JsonProperty("rules")]
        public List<RuleDataModel> Rules { get; set; }
    }
}