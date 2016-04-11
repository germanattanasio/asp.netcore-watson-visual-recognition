using Newtonsoft.Json;

namespace WatsonServices.Models.AlchemyVision
{
    public class ImageSceneTextResponse : ApiResponse
    {
        [JsonProperty("sceneText")]
        public string SceneText { get; set; }
        [JsonProperty("sceneTextLines")]
        public ImageSceneTextLine[] SceneTextLines { get; set; }
        // Number of internal API transactions that were required to satisfy the request
        [JsonProperty("totalTransactions")]
        public int TotalTransactions { get; set; }

        public ImageSceneTextResponse()
        {
            SceneTextLines = new ImageSceneTextLine[0];
        }
    }
}
