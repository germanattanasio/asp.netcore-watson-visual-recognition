using Newtonsoft.Json;

namespace VR.Models
{
    public class WatsonVRResponse
    {
        [JsonProperty("images")]
        public WatsonVRImage[] Images { get; set; }

        public WatsonVRResponse()
        {
            Images = new WatsonVRImage[0];
        }
    }
}
