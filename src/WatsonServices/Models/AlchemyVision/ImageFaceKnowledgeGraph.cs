using Newtonsoft.Json;

namespace WatsonServices.Models.AlchemyVision
{
    public class ImageFaceKnowledgeGraph
    {
        [JsonProperty("typeHierarchy")]
        public string TypeHierarchy { get; set; }
    }
}
