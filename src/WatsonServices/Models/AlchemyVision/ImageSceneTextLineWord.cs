using Newtonsoft.Json;

namespace WatsonServices.Models.AlchemyVision
{
    public class ImageSceneTextLineWord
    {
        [JsonProperty("confidence")]
        public double ConfidenceScore { get; set; }
        [JsonProperty("region")]
        public Region Region { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
