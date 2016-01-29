using System.Collections.Generic;

namespace VR.Models
{
    public class WatsonVRImageViewModel
    {
        public string ImageName { get; set; }
        public string Base64Image { get; set; }
        public IList<WatsonVRScoreViewModel> Scores { get; set; }

        public WatsonVRImageViewModel()
        {
            Scores = new List<WatsonVRScoreViewModel>();
        }
    }
}
