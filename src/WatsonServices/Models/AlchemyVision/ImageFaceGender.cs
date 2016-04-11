using Newtonsoft.Json;

namespace WatsonServices.Models.AlchemyVision
{
    public class ImageFaceGender
    {
        [JsonProperty("gender")]
        public string Gender { get; set; }
        [JsonProperty("score")]
        public double Score { get; set; }
    }
}
