using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using System;
using VisualRecognition.Services;
using WatsonServices.Extensions;

namespace VisualRecognition
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile("config.json", optional: true);
            Configuration = configBuilder.Build();

            // try to parse VCAP_SERVICES environment variable and overwrite any values from config.json
            string vcapServices = Environment.GetEnvironmentVariable("VCAP_SERVICES");
            if (vcapServices != null)
            {
                dynamic json = JsonConvert.DeserializeObject(vcapServices);
                // attempt to get Alchemy API credentials from VCAP_SERVICES
                if (json.alchemy_api != null)
                {
                    try
                    {
                        string apikey = json.alchemy_api[0].credentials.apikey;
                        string url = json.alchemy_api[0].credentials.url;
                        Configuration["alchemy_api:0:credentials:apikey"] = apikey;
                        Configuration["alchemy_api:0:credentials:url"] = url;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Could not parse VCAP_SERVICES Alchemy API credentials");
                    }
                }

                // attempt to get Visual Recognition credentials from VCAP_SERVICES
                if (json.visual_recognition != null)
                {
                    try
                    {
                        string password = json.visual_recognition[0].credentials.password;
                        string url = json.visual_recognition[0].credentials.url;
                        string username = json.visual_recognition[0].credentials.username;
                        Configuration["visual_recognition:0:credentials:password"] = password;
                        Configuration["visual_recognition:0:credentials:url"] = url;
                        Configuration["visual_recognition:0:credentials:username"] = username;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Could not parse VCAP_SERVICES Visual Recognition credentials");
                    }
                }
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // Add Watson services. See WatsonServices/WatsonExtensions.cs
            services.AddWatsonServices(Configuration);

            // register other services
            services.AddTransient<IFileEncoderService, Base64FileEncoderService>();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}