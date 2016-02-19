using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using VisualRecognition.Services;
using WatsonServices.Models;
using WatsonServices.Services;

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

            string vcapServices = System.Environment.GetEnvironmentVariable("VCAP_SERVICES");
            if (vcapServices != null)
            {
                dynamic json = JsonConvert.DeserializeObject(vcapServices);
                if (json.visual_recognition != null)
                {
                    string url = json.visual_recognition[0].credentials.url;
                    string username = json.visual_recognition[0].credentials.username;
                    string password = json.visual_recognition[0].credentials.password;
                    Configuration["visual_recognition:0:credentials:username"] = username;
                    Configuration["visual_recognition:0:credentials:password"] = password;
                    Configuration["visual_recognition:0:credentials:url"] = url;
                }
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // works with VCAP_SERVICES JSON value added to config.json when running locally,
            // and works with actual VCAP_SERVICES env var based on configuration set above when running in CF
            services.Configure<Credentials>(Configuration.GetSection("visual_recognition:0:credentials"));
            services.AddTransient<IFileEncoderService, Base64FileEncoderService>();
            services.AddTransient<IVisualRecognitionService, VisualRecognitionService>();
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