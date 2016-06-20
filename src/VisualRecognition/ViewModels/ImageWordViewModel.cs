using Newtonsoft.Json;

namespace VisualRecognition.ViewModels
{
    public class ImageWordViewModel
    {
        [JsonProperty("score")]
        public double ConfidenceScore { get; set; }
        [JsonProperty("location")]
        public ImageLocationViewModel Location { get; set; }
        [JsonProperty("word")]
        public string Word { get; set; }
        [JsonProperty("line_number")]
        public int LineNumber { get; set; }
    }
}
