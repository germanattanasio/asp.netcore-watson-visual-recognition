using Newtonsoft.Json;

namespace VisualRecognition.ViewModels
{
    public class ImageFaceIdentityViewModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("score")]
        public double ConfidenceScore { get; set; }
        [JsonProperty("type_hierarchy")]
        public string TypeHierarchy { get; set; }
    }
}