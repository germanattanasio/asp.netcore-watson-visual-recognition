using Newtonsoft.Json;

namespace VisualRecognition.Models
{
    public class Response
    {
        [JsonProperty("images")]
        public Image[] Images { get; set; }

        public Response()
        {
            Images = new Image[0];
        }
    }
}
