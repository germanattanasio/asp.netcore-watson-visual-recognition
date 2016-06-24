using Newtonsoft.Json;
using System.Collections.Generic;

namespace VisualRecognition.ViewModels
{
    public class ClassificationScoreViewModel
    {
        [JsonProperty("classifier_id")]
        public string ClassifierId { get; set; }
        [JsonProperty("name")]
        public string ClassifierName { get; set; }
        [JsonProperty("classes")]
        public ICollection<ClassResultViewModel> ClassResults { get; set; }

        public ClassificationScoreViewModel()
        {
            ClassResults = new ClassResultViewModel[0];
        }
    }
}
