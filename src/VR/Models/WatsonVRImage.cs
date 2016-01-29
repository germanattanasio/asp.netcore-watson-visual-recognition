using Newtonsoft.Json;

namespace VR.Models
{
    public class WatsonVRImage
    {
        [JsonProperty("image")]
        public string ImageName { get; set; }
        [JsonProperty("scores")]
        public WatsonVRScore[] Scores { get; set; }

        public WatsonVRImage()
        {
            Scores = new WatsonVRScore[0];
        }
    }
}
