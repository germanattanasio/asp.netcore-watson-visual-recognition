using Newtonsoft.Json;

namespace WatsonServices.Models.VisualRecognition
{
    public class ImageWord
    {
        [JsonProperty("score")]
        public double ConfidenceScore { get; set; }
        [JsonProperty("location")]
        public ImageLocation Location { get; set; }
        [JsonProperty("word")]
        public string Word { get; set; }
        [JsonProperty("line_number")]
        public int LineNumber { get; set; }
    }
}
