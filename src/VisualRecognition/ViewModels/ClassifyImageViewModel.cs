namespace VisualRecognition.ViewModels
{
    public class ClassifyImageViewModel
    {
        public string ImageSet { get; set; }
        public string Url { get; set; }
        // Base64 Encoded File
        public string ImageData { get; set; }
        public byte[] ImageByteContent { get; set; }
        public string ImageFileName { get; set; }
        public ClassifierViewModel Classifier { get; set; }
        public string[] ClassifierOwners { get; internal set; }

        public ClassifyImageViewModel()
        {
            ImageByteContent = new byte[0];
        }
    }
}
