using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WatsonServices.Services;

namespace WatsonServices.Extensions
{
    public static class WatsonExtensions
    {
        public static IServiceCollection AddWatsonServices(this IServiceCollection services, IConfiguration config)
        {
            Models.AlchemyAPICredentials alchemyCreds = new Models.AlchemyAPICredentials()
            {
                ApiEndPoint = config["alchemy_api:0:credentials:url"],
                ApiKey = config["alchemy_api:0:credentials:apikey"]
            };

            Models.VisualRecognition.Credentials vrCreds = new Models.VisualRecognition.Credentials()
            {
                Password = config["visual_recognition:0:credentials:password"],
                Url = config["visual_recognition:0:credentials:url"],
                Username = config["visual_recognition:0:credentials:username"]
            };

            // register service credentials and services if credentials are valid
            if (alchemyCreds.IsValid)
            {
                services.AddSingleton(typeof(Models.AlchemyAPICredentials), alchemyCreds);
                services.AddTransient<IAlchemyVisionService, AlchemyVisionService>();
            }

            if (vrCreds.IsValid)
            {
                services.AddSingleton(typeof(Models.VisualRecognition.Credentials), vrCreds);
                services.AddTransient<IVisualRecognitionService, VisualRecognitionService>();
            }

            return services;
        }
    }
}
