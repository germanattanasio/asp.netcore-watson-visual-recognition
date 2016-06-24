namespace WatsonServices.Models.VisualRecognition
{
    public class ClassPositiveExamples
    {
        public string ClassName { get; set; }
        public byte[] FileContents { get; internal set; }
        public string FileName { get; set; }
    }
}
