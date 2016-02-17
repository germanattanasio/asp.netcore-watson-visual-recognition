using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.Rendering;
using System.Collections.Generic;

namespace VisualRecognition.Models
{
    public class VisualRecognitionViewModel
    {
        public string ClassifierId { get; set; }
        public IEnumerable<SelectListItem> ClassifierIds { get; set; }
        public ICollection<ImageViewModel> ImageResults { get; set; }
        public IFormFile ImageUpload { get; set; }
        public string ImageUrl { get; set; }
        public int MaxScores { get; set; }

        public VisualRecognitionViewModel()
        {
            ClassifierIds = new List<SelectListItem>();
            ImageResults = new List<ImageViewModel>();
        }
    }
}
