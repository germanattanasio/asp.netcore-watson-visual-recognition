using Newtonsoft.Json;
using System.Collections.Generic;

namespace VisualRecognition.ViewModels
{
    public class ImageViewModel
    {
        [JsonProperty("image")]
        public string ImageName { get; set; }
        [JsonProperty("scores")]
        public ClassificationScoreViewModel[] Scores { get; set; }

        public ImageViewModel()
        {
            Scores = new ClassificationScoreViewModel[0];
        }
    }
}
