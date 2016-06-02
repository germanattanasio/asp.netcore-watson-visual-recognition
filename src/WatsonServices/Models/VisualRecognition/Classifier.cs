using Newtonsoft.Json;

namespace WatsonServices.Models.VisualRecognition
{
    public class Classifier
    {
        [JsonProperty("classifier_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ClassifierId { get; set; }
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        [JsonProperty("created", NullValueHandling = NullValueHandling.Ignore)]
        public string CreatedTime { get; set; }
        [JsonProperty("owner", NullValueHandling = NullValueHandling.Ignore)]
        public string Owner { get; set; }
        // status The training status of the new classifier.
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }
        // classes An array of classes that define a classifier.
        [JsonProperty("classes", NullValueHandling = NullValueHandling.Ignore)]
        public ClassifierClass[] Classes { get; set; }
        // explanation Omitted if the classifier trains successfully.If the classifier training fails, this might explain why training failed.
        [JsonProperty("explanation", NullValueHandling = NullValueHandling.Ignore)]
        public string Explanation { get; set; }
    }
}
