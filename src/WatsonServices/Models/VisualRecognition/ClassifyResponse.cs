using Newtonsoft.Json;

namespace WatsonServices.Models.VisualRecognition
{
    public class ClassifyResponse
    {
        [JsonProperty("images")]
        public Image[] Images { get; set; }

        public ClassifyResponse()
        {
            Images = new Image[0];
        }
    }
}
