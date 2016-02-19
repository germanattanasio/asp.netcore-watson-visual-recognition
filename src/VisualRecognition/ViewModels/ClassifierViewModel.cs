using System;

namespace VisualRecognition.ViewModels
{
    public class ClassifierViewModel
    {
        public string ClassifierId { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string Owner { get; set; }
    }
}
