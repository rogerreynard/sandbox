using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sitecore.PersonalizationGoogleAnalyticsIntegration.Models
{
    [Serializable]
    public class PersonalizedComponentImpressionModel
    {
        [JsonProperty("personalized_impression_data")]
        public List<PersonalizedImpressionDataModel> PersonalizedImpressionData { get; set; }
    }
}