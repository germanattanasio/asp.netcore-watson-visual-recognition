using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc.ModelBinding;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using VisualRecognition.Mappers;
using VisualRecognition.Services;
using WatsonServices.Models.VisualRecognition;

namespace VisualRecognition.ViewModels
{
    public class ClassifyImageViewModelBinder : IModelBinder
    {
        public async Task<ModelBindingResult> BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(ClassifyImageViewModel))
            {
                return ModelBindingResult.NoResult;
            }

            var result = new ClassifyImageViewModel();

            try
            {
                result.ImageData = (string)bindingContext.ValueProvider.GetValue("image_data");
                result.ImageSet = (string)bindingContext.ValueProvider.GetValue("use--example-images");
                result.Url = (string)bindingContext.ValueProvider.GetValue("url");

                Uri imageUri;
                string fileExt = "";
                bool isUri = Uri.TryCreate(result.Url, UriKind.Absolute, out imageUri);
                if (isUri)
                {
                    // download the file
                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync(result.Url);
                        if (!response.IsSuccessStatusCode)
                        {
                            // return 400 status
                            return ModelBindingResult.Failed("url");
                        }

                        fileExt = Path.GetExtension(result.Url);
                        result.ImageByteContent = await response.Content.ReadAsByteArrayAsync();
                    }
                }
                else if (!string.IsNullOrEmpty(result.Url))
                {
                    // relative path given for a sample image
                    try
                    {
                        fileExt = Path.GetExtension(result.Url);
                        var hostingEnvironmentService = (IHostingEnvironment)bindingContext.OperationBindingContext
                            .HttpContext.ApplicationServices.GetService(typeof(IHostingEnvironment));

                        result.ImageByteContent = File.ReadAllBytes(
                            Path.Combine(hostingEnvironmentService.WebRootPath, result.Url));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                }
                else if (!string.IsNullOrEmpty(result.ImageData))
                {
                    string fileType = "";
                    string encodedFile = "";
                    try
                    {
                        var fileEncoderService = (IFileEncoderService)bindingContext.OperationBindingContext.HttpContext
                            .ApplicationServices.GetService(typeof(IFileEncoderService));

                        string[] encodedFileParts = result.ImageData.Split(';');
                        encodedFile = encodedFileParts[1].Split(',')[1];
                        fileType = encodedFileParts[0].Split('/')[1];
                        fileExt = "." + fileType;
                        result.ImageByteContent = await fileEncoderService.DecodeFileAsync(encodedFile);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                }
                if (!string.IsNullOrEmpty(fileExt))
                {
                    result.ImageFileName = result.ImageSet + fileExt;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return ModelBindingResult.Failed(bindingContext.FieldName);
            }

            try
            {
                string classifierJson = bindingContext.OperationBindingContext.HttpContext.Request.Cookies["classifier"];
                if (classifierJson != null)
                {
                    result.Classifier = ClassifierMapper.Map(
                        Newtonsoft.Json.JsonConvert.DeserializeObject<Classifier>(classifierJson));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            return ModelBindingResult.Success(bindingContext.FieldName, result);
        }
    }
}
