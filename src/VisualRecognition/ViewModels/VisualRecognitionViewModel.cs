using Newtonsoft.Json;
using System.Collections.Generic;

namespace VisualRecognition.ViewModels
{
    public class VisualRecognitionViewModel
    {
        [JsonProperty("images")]
        public ICollection<ImageViewModel> Images { get; set; }

        public VisualRecognitionViewModel()
        {
            Images = new ImageViewModel[0];
        }
    }
}
