using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.Rendering;
using System.Collections.Generic;

namespace VR.Models
{
    public class WatsonVRViewModel
    {
        public string ClassifierId { get; set; }
        public IEnumerable<SelectListItem> ClassifierIds { get; set; }
        public ICollection<WatsonVRImageViewModel> ImageResults { get; set; }
        public IFormFile ImageUpload { get; set; }
        public string ImageUrl { get; set; }
        public int MaxScores { get; set; }

        public WatsonVRViewModel()
        {
            ClassifierIds = new List<SelectListItem>();
            ImageResults = new List<WatsonVRImageViewModel>();
        }
    }
}
