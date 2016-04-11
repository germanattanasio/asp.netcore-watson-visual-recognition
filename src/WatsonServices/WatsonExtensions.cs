#if DNXCORE50 || DNX451
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
#endif
using WatsonServices.Services;

namespace WatsonServices.Extensions
{
    public static class WatsonExtensions
    {
        // this function should only be compiled if targetting DNXCORE50 or DNX451
        #if DNXCORE50 || DNX451
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
                services.AddInstance(typeof(Models.AlchemyAPICredentials), alchemyCreds);
                services.AddTransient<IAlchemyVisionService, AlchemyVisionService>();
            }

            if (vrCreds.IsValid)
            {
                services.AddInstance(typeof(Models.VisualRecognition.Credentials), vrCreds);
                services.AddTransient<IVisualRecognitionService, VisualRecognitionService>();
            }

            return services;
        }
        #endif
    }
}
