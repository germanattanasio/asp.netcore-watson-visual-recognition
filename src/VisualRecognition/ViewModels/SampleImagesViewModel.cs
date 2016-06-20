using System;

namespace VisualRecognition.ViewModels
{
    public class SampleImagesViewModel
    {
        public string BundleId { get; set; }
        public string[] BundleImages { get; set; }
        public string ClassName { get; private set; }
        public SampleImagesClassEnum Class
        {
            get
            {
                return (SampleImagesClassEnum)Enum.Parse(typeof(SampleImagesClassEnum), ClassName);
            }
            set
            {
                ClassName = value.ToString().ToLowerInvariant();
            }
        }

        public SampleImagesViewModel()
        {
            ClassName = SampleImagesClassEnum.Use.ToString().ToLowerInvariant();
            BundleImages = new string[0];
        }
    }
}
