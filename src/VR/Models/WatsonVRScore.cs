using Newtonsoft.Json;

namespace VR.Models
{
    public class WatsonVRScore
    {
        [JsonProperty("classifier_id")]
        public string ClassifierId { get; set; }
        [JsonProperty("name")]
        public string ClassifierName { get; set; }
        [JsonProperty("score")]
        public double Score { get; set; }
    }
}
