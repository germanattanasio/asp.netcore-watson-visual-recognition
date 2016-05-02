using Newtonsoft.Json;

namespace VisualRecognition.ViewModels
{
    public class ClassificationScoreViewModel
    {
        [JsonProperty("classifier_id")]
        public string ClassifierId { get; set; }
        [JsonProperty("name")]
        public string ClassifierName { get; set; }
        [JsonProperty("score")]
        public string Score { get; set; }
    }
}
