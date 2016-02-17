using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.Rendering;
using System.Collections.Generic;

namespace VisualRecognition.Models
{
    public class ManageClassifierViewModel
    {
        public ClassifierActionType ActionType { get; set; }
        public string ClassifierId { get; set; }
        public IEnumerable<SelectListItem> ClassifierIds { get; set; }
        public string ClassifierName { get; set; }
        public IFormFile NegativeExamples { get; set; }
        public IFormFile PositiveExamples { get; set; }
        public bool Success { get; set; }

        public ManageClassifierViewModel()
        {
            ActionType = ClassifierActionType.None;
            ClassifierIds = new List<SelectListItem>();
        }
    }

    public enum ClassifierActionType
    {
        Create,
        Delete,
        None
    }
}
