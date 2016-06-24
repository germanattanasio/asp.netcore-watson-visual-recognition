using Newtonsoft.Json;

namespace VisualRecognition.ViewModels
{
    public class ImageFaceGenderViewModel
    {
        [JsonProperty("gender")]
        public string Gender { get; set; }
        [JsonProperty("score")]
        public double Score { get; set; }
    }
}