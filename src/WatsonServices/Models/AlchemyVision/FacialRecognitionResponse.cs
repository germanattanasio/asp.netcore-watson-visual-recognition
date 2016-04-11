using Newtonsoft.Json;

namespace WatsonServices.Models.AlchemyVision
{
    public class FacialRecognitionResponse : ApiResponse
    {
        [JsonProperty("imageFaces")]
        public ImageFace[] ImageFaces { get; set; }
        // Number of internal API transactions that were required to satisfy the request
        [JsonProperty("totalTransactions")]
        public int TotalTransactions { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }

        public FacialRecognitionResponse()
        {
            ImageFaces = new ImageFace[0];
        }
    }
}
