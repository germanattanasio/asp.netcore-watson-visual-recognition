namespace VR.Models
{
    public class WqaModel
    {
        public string question { get; set; }
        public string answer { get; set; }
        public int[] id { get; set; }
        public string[] text { get; set; }
        public float[] confidence { get; set; }
    }
}
