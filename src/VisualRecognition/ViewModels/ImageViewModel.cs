using Newtonsoft.Json;
using System.Collections.Generic;

namespace VisualRecognition.ViewModels
{
    public class ImageViewModel
    {
        [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore)]
        public string ImageName { get; set; }
        [JsonProperty("resolved_url", NullValueHandling = NullValueHandling.Ignore)]
        public string ResolvedUrl { get; set; }
        [JsonProperty("source_url", NullValueHandling = NullValueHandling.Ignore)]
        public string SourceUrl { get; set; }
        [JsonProperty("classifiers", NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<ClassificationScoreViewModel> Scores { get; set; }
        [JsonProperty("faces", NullValueHandling = NullValueHandling.Ignore)]
        public ImageFaceViewModel[] Faces { get; set; }
        [JsonProperty("words", NullValueHandling = NullValueHandling.Ignore)]
        public ImageWordViewModel[] Words { get; set; }
        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string ImageText { get; set; }
    }
}
