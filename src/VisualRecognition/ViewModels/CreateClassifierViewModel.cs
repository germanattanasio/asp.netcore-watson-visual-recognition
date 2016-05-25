using Microsoft.AspNetCore.Http;

namespace VisualRecognition.ViewModels
{
    public class CreateClassifierViewModel
    {
        public string[] Negatives { get; set; }
        public string[] Positives { get; set; }
        public string Name { get; set; }
    }
}
