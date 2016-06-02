using Newtonsoft.Json;

namespace WatsonServices.Models.VisualRecognition
{
    public class ImageFaceAge
    {
        [JsonProperty("min")]
        public int MinimumAge { get; set; }
        [JsonProperty("max")]
        public int MaximumAge { get; set; }
        [JsonProperty("score")]
        public double Score { get; set; }
    }
}
