using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        private const string CookiesClassifierKey = "classifier";
        private const string ImageDataKey = "image_data";
        private const string UseImageSetKey = "use--example-images";
        private const string TestImageSetKey = "test--example-images";
        private const string UrlKey = "url";

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(ClassifyImageViewModel))
            {
                bindingContext.Result = null;
                return;
            }

            var result = new ClassifyImageViewModel();

            try
            {
                result.ImageData = (string)bindingContext.ValueProvider.GetValue(ImageDataKey);
                result.ImageSet = ((string)bindingContext.ValueProvider.GetValue(UseImageSetKey)) ??
                    ((string)bindingContext.ValueProvider.GetValue(TestImageSetKey));
                result.Url = (string)bindingContext.ValueProvider.GetValue(UrlKey);

                Uri imageUri;
                string fileExt = "";
                result.IsUrl = Uri.TryCreate(result.Url, UriKind.Absolute, out imageUri);
                if (result.IsUrl)
                {
                    // download the file
                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync(result.Url);
                        if (!response.IsSuccessStatusCode)
                        {
                            // return 400 status
                            bindingContext.Result = ModelBindingResult.Failed("url");
                            return;
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
                            .HttpContext.RequestServices.GetService(typeof(IHostingEnvironment));

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
                            .RequestServices.GetService(typeof(IFileEncoderService));

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
                bindingContext.Result = ModelBindingResult.Failed(bindingContext.FieldName);
                return;
            }

            try
            {
                string classifierJson = bindingContext.OperationBindingContext.HttpContext.Request.Cookies[CookiesClassifierKey];
                // only use this classifier if we're looking at test images
                if (classifierJson != null &&
                    !string.IsNullOrEmpty(((string)bindingContext.ValueProvider.GetValue(TestImageSetKey))))
                {
                    result.Classifier = ClassifierMapper.Map(
                        Newtonsoft.Json.JsonConvert.DeserializeObject<Classifier>(classifierJson));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            bindingContext.Result = ModelBindingResult.Success(bindingContext.FieldName, result);
            return;
        }
    }
}
