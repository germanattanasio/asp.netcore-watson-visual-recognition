using Newtonsoft.Json;

namespace WatsonServices.Models.AlchemyVision
{
    public class ImageSceneTextLine
    {
        [JsonProperty("confidence")]
        public double ConfidenceScore { get; set; }
        [JsonProperty("region")]
        public Region Region { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("words")]
        public ImageSceneTextLineWord[] Words { get; set; }

        public ImageSceneTextLine()
        {
            Words = new ImageSceneTextLineWord[0];
        }
    }
}
