using Newtonsoft.Json;

namespace VisualRecognition.Models
{
    public class Image
    {
        [JsonProperty("image")]
        public string ImageName { get; set; }
        [JsonProperty("scores")]
        public ClassificationScore[] Scores { get; set; }

        public Image()
        {
            Scores = new ClassificationScore[0];
        }
    }
}
