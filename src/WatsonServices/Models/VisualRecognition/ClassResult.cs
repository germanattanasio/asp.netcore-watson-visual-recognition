using Newtonsoft.Json;

namespace WatsonServices.Models.VisualRecognition
{
    public class ClassResult
    {
        [JsonProperty("class")]
        public string ClassId { get; set; }
        [JsonProperty("score")]
        public double Score { get; set; }
        [JsonProperty("type_hierarchy", NullValueHandling = NullValueHandling.Ignore)]
        public string TypeHierarchy { get; set; }
    }
}
