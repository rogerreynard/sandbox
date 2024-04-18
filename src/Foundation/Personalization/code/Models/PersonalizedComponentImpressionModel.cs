using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Foundation.Personalization.Models
{
    [Serializable]
    public class PersonalizedComponentImpressionModel
    {
        [JsonProperty("event")]
        public string Event => "personalized_component_impression";
        [JsonProperty("personalized_impression_data")]
        public List<PersonalizedImpressionDataModel> PersonalizedImpressionData { get; set; }
    }
}