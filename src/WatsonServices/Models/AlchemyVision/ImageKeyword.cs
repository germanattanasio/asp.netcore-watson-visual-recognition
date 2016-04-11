using Newtonsoft.Json;

namespace WatsonServices.Models.AlchemyVision
{
    public class ImageKeyword
    {
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("score")]
        public double? ConfidenceScore { get; set; }
    }
}
