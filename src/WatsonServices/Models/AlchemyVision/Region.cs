using Newtonsoft.Json;

namespace WatsonServices.Models.AlchemyVision
{
    public class Region
    {
        [JsonProperty("height")]
        public int Height { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("x")]
        public int PositionX { get; set; }
        [JsonProperty("y")]
        public int PositionY { get; set; }
    }
}
