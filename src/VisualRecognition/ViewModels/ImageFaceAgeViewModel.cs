using Newtonsoft.Json;

namespace VisualRecognition.ViewModels
{
    public class ImageFaceAgeViewModel
    {
        [JsonProperty("min")]
        public int MinimumAge { get; set; }
        [JsonProperty("max")]
        public int MaximumAge { get; set; }
        [JsonProperty("score")]
        public double Score { get; set; }
    }
}