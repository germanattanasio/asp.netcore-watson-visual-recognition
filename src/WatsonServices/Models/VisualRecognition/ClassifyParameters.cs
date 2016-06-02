using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace WatsonServices.Models.VisualRecognition
{
    internal class ClassifyParameters
    {
        [JsonProperty("classifier_ids", NullValueHandling = NullValueHandling.Ignore)]
        public string[] ClassifierIds { get; internal set; }
        [JsonProperty("owners", NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<ClassifierOwner> Owners { get; internal set; }
        [JsonProperty("threshold", NullValueHandling = NullValueHandling.Ignore)]
        public string Threshold { get; internal set; }
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }
        public bool HasContent
        {
            get
            {
                return !string.IsNullOrEmpty(Url) ||
                       !string.IsNullOrEmpty(Threshold) ||
                       (Owners?.Any() == true) ||
                       (ClassifierIds?.Any() == true);
            }
        }
    }
}
