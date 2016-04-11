using Newtonsoft.Json;

namespace WatsonServices.Models.AlchemyVision
{
    public class ImageFaceIdentity
    {
        [JsonProperty("disambiguated")]
        public ImageFaceIdentityDisambiguated Disambiguated { get; set; }
        [JsonProperty("knowledgeGraph")]
        public ImageFaceKnowledgeGraph KnowledgeGraph { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("score")]
        public double ConfidenceScore { get; set; }
    }
}
