using Newtonsoft.Json;

namespace WatsonServices.Models.AlchemyVision
{
    public class ApiStatus
    {
        [JsonProperty("status")]
        public string StatusCode { get; set; }
        [JsonProperty("statusInfo")]
        public string StatusDetails { get; set; }
    }
}
