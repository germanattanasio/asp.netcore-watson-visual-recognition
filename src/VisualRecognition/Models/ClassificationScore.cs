using Newtonsoft.Json;

namespace VisualRecognition.Models
{
    public class ClassificationScore
    {
        [JsonProperty("classifier_id")]
        public string ClassifierId { get; set; }
        [JsonProperty("name")]
        public string ClassifierName { get; set; }
        [JsonProperty("score")]
        public double Score { get; set; }
    }
}
