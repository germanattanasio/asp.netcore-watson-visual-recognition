using Newtonsoft.Json;

namespace WatsonServices.Models.VisualRecognition
{
    public class ClassifyResponse
    {
        [JsonProperty("images", NullValueHandling = NullValueHandling.Ignore)]
        public Image[] Images { get; set; }
        [JsonProperty("images_processed", NullValueHandling = NullValueHandling.Ignore)]
        public int ImagesProcessed { get; set; }
        [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
        public ErrorResponse Error { get; set; }
    }
}
