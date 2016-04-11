using Newtonsoft.Json;

namespace WatsonServices.Models.AlchemyVision
{
    public class ImageKeywordResponse : ApiResponse
    {
        [JsonProperty("imageKeywords")]
        public ImageKeyword[] ImageKeywords { get; set; }
        // Number of internal API transactions that were required to satisfy the request
        [JsonProperty("totalTransactions")]
        public int TotalTransactions { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }

        public ImageKeywordResponse()
        {
            ImageKeywords = new ImageKeyword[0];
        }
    }
}
