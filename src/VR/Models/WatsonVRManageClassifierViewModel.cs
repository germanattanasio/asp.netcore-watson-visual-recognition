using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.Rendering;
using System.Collections.Generic;

namespace VR.Models
{
    public class WatsonVRManageClassifierViewModel
    {
        public WatsonVRClassifierAction ActionType { get; set; }
        public string ClassifierId { get; set; }
        public IEnumerable<SelectListItem> ClassifierIds { get; set; }
        public string ClassifierName { get; set; }
        public IFormFile NegativeExamples { get; set; }
        public IFormFile PositiveExamples { get; set; }
        public bool Success { get; set; }

        public WatsonVRManageClassifierViewModel()
        {
            ActionType = WatsonVRClassifierAction.None;
            ClassifierIds = new List<SelectListItem>();
        }
    }

    public enum WatsonVRClassifierAction
    {
        Create,
        Delete,
        None
    }
}
