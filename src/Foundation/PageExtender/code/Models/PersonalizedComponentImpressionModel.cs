using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Foundation.PageExtender.Models
{
    [Serializable]
    public class PersonalizedComponentImpressionModel
    {
        [JsonProperty("personalized_impression_data")]
        public List<PersonalizedImpressionDataModel> PersonalizedImpressionData { get; set; }
    }
}