using Newtonsoft.Json;

namespace WatsonServices.Models.VisualRecognition
{
    public class Image
    {
        [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore)]
        public string ImageName { get; set; }
        [JsonProperty("resolved_url", NullValueHandling = NullValueHandling.Ignore)]
        public string ResolvedUrl { get; set; }
        [JsonProperty("source_url", NullValueHandling = NullValueHandling.Ignore)]
        public string SourceUrl { get; set; }
        [JsonProperty("classifiers", NullValueHandling = NullValueHandling.Ignore)]
        public ClassificationScore[] Scores { get; set; }
        [JsonProperty("faces", NullValueHandling = NullValueHandling.Ignore)]
        public ImageFace[] Faces { get; set; }
        [JsonProperty("words", NullValueHandling = NullValueHandling.Ignore)]
        public ImageWord[] Words { get; set; }
        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string ImageText { get; set; }
    }
}
