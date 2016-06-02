using System;
using Newtonsoft.Json;

namespace WatsonServices.Models.VisualRecognition
{
    public class ClassifierOwnerJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ClassifierOwner);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            try
            {
                // "IBM" should be capitalized but "me" should be lowercase
                writer.WriteValue(
                    (ClassifierOwner)value == ClassifierOwner.IBM 
                    ? ((ClassifierOwner)value).ToString() 
                    : ((ClassifierOwner)value).ToString().ToLowerInvariant());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
