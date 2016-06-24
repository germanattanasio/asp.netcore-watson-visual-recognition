using Newtonsoft.Json;

namespace VisualRecognition.ViewModels
{
    public class ClassResultViewModel
    {
        [JsonProperty("class")]
        public string ClassId { get; set; }
        [JsonProperty("score")]
        public double Score { get; set; }
        [JsonProperty("type_hierarchy", NullValueHandling = NullValueHandling.Ignore)]
        public string TypeHierarchy { get; set; }
    }
}
