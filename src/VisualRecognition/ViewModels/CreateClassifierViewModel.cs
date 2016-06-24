using System.Collections.Generic;

namespace VisualRecognition.ViewModels
{
    public class CreateClassifierViewModel
    {
        public IList<string> Bundles { get; set; }
        public string[] Names { get; set; }
        public string Kind { get; set; }
        public string Name { get; set; }
    }
}
