using Newtonsoft.Json;

namespace WatsonServices.Models.VisualRecognition
{
    [JsonConverter(typeof(ClassifierOwnerJsonConverter))]
    public enum ClassifierOwner
    {
        IBM,
        Me
    }
}