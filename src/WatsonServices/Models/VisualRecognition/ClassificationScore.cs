using Newtonsoft.Json;
using System.Collections.Generic;

namespace WatsonServices.Models.VisualRecognition
{
    public class ClassificationScore
    {
        [JsonProperty("classifier_id")]
        public string ClassifierId { get; set; }
        [JsonProperty("name")]
        public string ClassifierName { get; set; }
        [JsonProperty("classes")]
        public ICollection<ClassResult> ClassResults { get; set; }

        public ClassificationScore()
        {
            ClassResults = new ClassResult[0];
        }
    }
}
