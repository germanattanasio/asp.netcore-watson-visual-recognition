using Newtonsoft.Json;
using System.Net;

namespace WatsonServices.Models.AlchemyVision
{
    public class ApiResponse
    {
        [JsonConverter(typeof(ApiStatusJsonConverter))]
        [JsonProperty("status")]
        public ApiStatus Status { get; set; }
        [JsonProperty("usage")]
        public string TermsOfUse { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
    }
}
