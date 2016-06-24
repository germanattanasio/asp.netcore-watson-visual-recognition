using Newtonsoft.Json;

namespace WatsonServices.Models.VisualRecognition
{
    public class ErrorResponse
    {
        [JsonProperty("code")]
        public string ErrorCode { get; set; }
        [JsonProperty("error_id")]
        public string ErrorId { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
