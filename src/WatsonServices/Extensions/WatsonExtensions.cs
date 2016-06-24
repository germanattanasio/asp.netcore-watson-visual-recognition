using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WatsonServices.Services;

namespace WatsonServices.Extensions
{
    public static class WatsonExtensions
    {
        public static IServiceCollection AddWatsonServices(this IServiceCollection services, IConfiguration config)
        {
            Models.WatsonVisionCombinedCredentials watsonVisionCreds = new Models.WatsonVisionCombinedCredentials()
            {
                ApiEndPoint = config["watson_vision_combined:0:credentials:url"],
                ApiKey = config["watson_vision_combined:0:credentials:api_key"]
            };

            // register service credentials and services if credentials are valid
            if (watsonVisionCreds.IsValid)
            {
                services.AddSingleton(typeof(Models.WatsonVisionCombinedCredentials), watsonVisionCreds);
                services.AddTransient<IVisualRecognitionService, VisualRecognitionService>();
            }

            return services;
        }

        public static IConfigurationBuilder AddVcapServices(this IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Add(new VcapServicesConfigurationSource());
            return configurationBuilder;
        }
    }
}
