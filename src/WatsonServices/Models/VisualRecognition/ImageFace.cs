using Newtonsoft.Json;

namespace WatsonServices.Models.VisualRecognition
{
    public class ImageFace
    {
        [JsonProperty("age")]
        public ImageFaceAge Age { get; set; }
        [JsonProperty("gender")]
        public ImageFaceGender Gender { get; set; }
        [JsonProperty("identity")]
        public ImageFaceIdentity Identity { get; set; }
        [JsonProperty("face_location")]
        public ImageLocation Location { get; set; }
    }
}
