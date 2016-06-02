using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WatsonServices.Extensions
{
    public class VcapServicesConfigurationProvider : ConfigurationProvider
    {
        public VcapServicesConfigurationProvider(VcapServicesConfigurationSource source) : base() { }

        public override void Load()
        {
            Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            string vcapServices = Environment.GetEnvironmentVariable("VCAP_SERVICES");
            if (vcapServices != null)
            {
                var jObject = JObject.Parse(vcapServices);
                Load(jObject);
            }
        }

        private void Load(JToken jToken)
        {
            foreach (JToken token in jToken.Values())
            {
                if (token.Type == JTokenType.String)
                {
                    var key = ConfigurationPath.Combine(GetPathTokens(token.Path));
                    Data[key] = ((JValue)token).Value as string;
                } else
                {
                    Load(token);
                }
            }
        }

        private IEnumerable<string> GetPathTokens(string path)
        {
            var result = new List<string>();
            var tokens = path?.Split('.').Select(m => m.TrimEnd(']')).ToList();
            foreach (var token in tokens)
            {
                var arrayPosition = token.IndexOf('[');
                if (arrayPosition < 0)
                {
                    result.Add(token);
                }
                else
                {
                    result.Add(token.Remove(arrayPosition));
                    AddArrayTokens(result, token.Substring(arrayPosition).Trim('[',']'));
                }
            }

            return result;
        }

        private void AddArrayTokens(List<string> result, string token)
        {
            var tokens = token?.Split('[').Select(m => m.TrimEnd(']')).ToList();
            foreach (var element in tokens)
            {
                result.Add(element);
            }
        }
    }
}
