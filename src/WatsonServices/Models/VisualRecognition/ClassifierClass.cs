using Newtonsoft.Json;

namespace WatsonServices.Models.VisualRecognition
{
    public class ClassifierClass
    {
        // class The name of the class.
        [JsonProperty("class")]
        public string Name { get; set; }
    }
}
