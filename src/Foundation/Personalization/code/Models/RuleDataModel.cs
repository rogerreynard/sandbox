using System;
using Newtonsoft.Json;

namespace Foundation.Personalization.Models
{
    [Serializable]
    public class RuleDataModel
    {
        [JsonProperty("rule_name")]
        public string RuleName { get; set; }
        [JsonProperty("rule_order")]
        public string RuleOrder { get; set; }
        [JsonProperty("action_state")]
        public string ActionState { get; set; }
        [JsonProperty("is_applied")]
        public string IsApplied { get; set; }
    }
}