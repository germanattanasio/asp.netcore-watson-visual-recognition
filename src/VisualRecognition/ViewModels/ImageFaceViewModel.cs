using Newtonsoft.Json;

namespace VisualRecognition.ViewModels
{
    public class ImageFaceViewModel
    {
        [JsonProperty("age", NullValueHandling = NullValueHandling.Ignore)]
        public ImageFaceAgeViewModel Age { get; set; }
        [JsonProperty("gender", NullValueHandling = NullValueHandling.Ignore)]
        public ImageFaceGenderViewModel Gender { get; set; }
        [JsonProperty("identity", NullValueHandling = NullValueHandling.Ignore)]
        public ImageFaceIdentityViewModel Identity { get; set; }
        [JsonProperty("face_location")]
        public ImageLocationViewModel Location { get; set; }
    }
}
