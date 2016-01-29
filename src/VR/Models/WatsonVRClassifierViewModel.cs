using System;

namespace VR.Models
{
    public class WatsonVRClassifierViewModel
    {
        public string ClassifierId { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string Owner { get; set; }
    }
}
