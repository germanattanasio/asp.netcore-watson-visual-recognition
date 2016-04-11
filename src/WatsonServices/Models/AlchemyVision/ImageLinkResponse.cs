using Newtonsoft.Json;

namespace WatsonServices.Models.AlchemyVision
{
    public class ImageLinkResponse : ApiResponse
    {
        [JsonProperty("image")]
        public string ImageUrl { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
