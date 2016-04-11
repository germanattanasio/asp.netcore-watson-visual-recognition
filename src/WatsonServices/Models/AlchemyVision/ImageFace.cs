using Newtonsoft.Json;

namespace WatsonServices.Models.AlchemyVision
{
    public class ImageFace
    {
        [JsonProperty("age")]
        public ImageFaceAge Age { get; set; }
        [JsonProperty("gender")]
        public ImageFaceGender Gender { get; set; }
        [JsonProperty("positionX")]
        public int PositionX { get; set; }
        [JsonProperty("positionY")]
        public int PositionY { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
        [JsonProperty("identity")]
        public ImageFaceIdentity Identity { get; set; }
    }
}
