using Newtonsoft.Json;

namespace WatsonServices.Models.AlchemyVision
{
    public class ImageFaceIdentityDisambiguated
    {
        [JsonProperty("dbpedia")]
        public string DbPedia { get; set; }
        [JsonProperty("freebase")]
        public string Freebase { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("opencyc")]
        public string OpenCyc { get; set; }
        [JsonProperty("subType")]
        public string[] SubType { get; set; }
        [JsonProperty("website")]
        public string Website { get; set; }
        [JsonProperty("yago")]
        public string Yago { get; set; }
    }
}
