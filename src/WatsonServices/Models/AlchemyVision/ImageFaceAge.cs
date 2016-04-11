using Newtonsoft.Json;

namespace WatsonServices.Models.AlchemyVision
{
    public class ImageFaceAge
    {
        [JsonProperty("ageRange")]
        public string AgeRange { get; set; }
        [JsonProperty("score")]
        public double Score { get; set; }
    }
}
