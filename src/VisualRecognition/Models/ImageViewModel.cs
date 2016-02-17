using System.Collections.Generic;

namespace VisualRecognition.Models
{
    public class ImageViewModel
    {
        public string ImageName { get; set; }
        public string Base64Image { get; set; }
        public IList<ClassificationScoreViewModel> Scores { get; set; }

        public ImageViewModel()
        {
            Scores = new List<ClassificationScoreViewModel>();
        }
    }
}
