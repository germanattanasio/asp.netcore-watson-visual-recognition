using Newtonsoft.Json;
using System;

namespace WatsonServices.Models.AlchemyVision
{
    public class ApiStatusJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ApiStatus);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            ApiStatus returnValue = new ApiStatus();
            try
            {
                if (reader.TokenType == JsonToken.StartObject)
                {
                    returnValue = serializer.Deserialize<ApiStatus>(reader);
                }
                else if (reader.TokenType == JsonToken.String)
                {
                    returnValue.StatusCode = serializer.Deserialize<string>(reader);
                }
            }
            catch (Exception)
            {
                // invalid json data
                return null;
            }

            return returnValue;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
