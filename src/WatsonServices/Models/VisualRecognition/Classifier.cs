using Newtonsoft.Json;

namespace WatsonServices.Models.VisualRecognition
{
    public class Classifier
    {
        [JsonProperty("classifier_id")]
        public string ClassifierId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("created")]
        public string CreatedTime { get; set; }
        [JsonProperty("owner")]
        public string Owner { get; set; }
    }
}
