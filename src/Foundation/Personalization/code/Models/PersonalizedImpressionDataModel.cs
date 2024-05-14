using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Foundation.Personalization.Models
{
    [Serializable]
    public class PersonalizedImpressionDataModel
    {
        [JsonProperty("user_name")]
        public string UserName { get; set; }
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
        public string DataSourcePath { get; set; }
        [JsonProperty("datasource_id")]
        public string DataSourceID { get; set; }
        [JsonProperty("card_list")]
        public List<HasPatternConditionModel> CardList { get; set; }
    }
}