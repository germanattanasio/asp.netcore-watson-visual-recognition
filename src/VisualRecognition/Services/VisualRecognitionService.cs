using Microsoft.Extensions.OptionsModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VisualRecognition.Mappers;
using VisualRecognition.Models;

namespace VisualRecognition.Services
{
    public class VisualRecognitionService : IVisualRecognitionService
    {
        private readonly HashSet<string> _acceptedImageTypes;
        private bool learningOptOut;
        private readonly Credentials _vrCreds;
        private const string VersionReleaseDate = "2015-12-02";

        // Specifies whether or not to share data with Watson for learning purposes
        public bool ShareData
        {
            get
            {
                return !learningOptOut;
            }
            set
            {
                // if ShareData is set true, don't opt out of sharing
                learningOptOut = !value;
            }
        }

        public VisualRecognitionService(IOptions<Credentials> optionsAccessor)
        {
            var creds = optionsAccessor.Value;
            if (creds != null)
            {
                _vrCreds = new Credentials
                {
                    Password = creds.Password,
                    Url = creds.Url,
                    Username = creds.Username
                };
            }
            if (_vrCreds == null || _vrCreds.Username == null || _vrCreds.Password == null || _vrCreds.Url == null)
            {
                throw new Exception("Missing Watson VR service credentials");
            }

            _acceptedImageTypes = new HashSet<string>()
            {
                ".jpg",
                ".jpeg",
                ".png"
            };
        }

        public async Task<VisualRecognitionViewModel> ClassifyAsync(string filePath, int? maxScores = null, params string[] classifierIds)
        {
            VisualRecognitionViewModel viewModel = new VisualRecognitionViewModel();
            using (var client = VrClient())
            {
                try
                {
                    byte[] buffer = File.ReadAllBytes(filePath);
                    var request = new MultipartFormDataContent();

                    var imageContent = new ByteArrayContent(buffer);
                    imageContent.Headers.ContentType =
                        MediaTypeHeaderValue.Parse("multipart/form-data");
                    var filename = Path.GetFileName(filePath);
                    var base64Images = new Dictionary<string, string>();
                    Task extractImages = null;
                    if (Path.GetExtension(filePath).ToLowerInvariant() != ".zip")
                    {
                        base64Images.Add(filename, Convert.ToBase64String(File.ReadAllBytes(filePath)));
                    }
                    else
                    {
                        extractImages = new TaskFactory().StartNew(() =>
                        {
                            var fileName = Path.GetFileNameWithoutExtension(filePath);
                            var folder = Path.GetDirectoryName(filePath);
                            var newTempFolder = Path.Combine(folder, fileName);
                            System.IO.Compression.ZipFile.ExtractToDirectory(filePath, newTempFolder);
                            foreach (var file in Directory.GetFiles(newTempFolder)
                                .Where(m => _acceptedImageTypes.Contains(Path.GetExtension(m).ToLowerInvariant())).ToList())
                            {
                                base64Images.Add(Path.GetFileName(file), Convert.ToBase64String(File.ReadAllBytes(file)));
                            }
                            foreach (var file in Directory.GetFiles(newTempFolder))
                            {
                                File.Delete(file);
                            }
                            Directory.Delete(newTempFolder);
                        });
                    }

                    if (classifierIds != null)
                    {
                        var serializedClassifiers = "{\"classifier_ids\":" + 
                            Newtonsoft.Json.JsonConvert.SerializeObject(classifierIds) + "}";
                        request.Add(new StringContent(serializedClassifiers), "classifier_ids");
                    }
                    
                    request.Add(imageContent, "images_file", filename);

                    var response = await client.PostAsync("api/v2/classify?version=" + VersionReleaseDate, request);
                    var msg = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase);
                    if (response.IsSuccessStatusCode)
                    {
                        var model = await response.Content.ReadAsAsync<Response>();
                        if (extractImages != null)
                        {
                            Task.WaitAll(extractImages);
                        }
                        ImagesMapper.Map(model, viewModel, base64Images, maxScores);
                    }
                    else
                    {
                        Console.WriteLine(msg);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
            return viewModel;
        }

        public async Task<ClassifierViewModel> CreateClassifierAsync(string positiveExamplesPath, string negativeExamplesPath, string classifierName)
        {
            ClassifierViewModel viewModel = null;
            using (var client = VrClient())
            {
                try
                {
                    byte[] positiveBuffer = File.ReadAllBytes(positiveExamplesPath);
                    byte[] negativeBuffer = File.ReadAllBytes(negativeExamplesPath);

                    var request = new MultipartFormDataContent();

                    var positiveImagesContent = new ByteArrayContent(positiveBuffer);
                    positiveImagesContent.Headers.ContentType =
                        MediaTypeHeaderValue.Parse("multipart/form-data");

                    var negativeImagesContent = new ByteArrayContent(negativeBuffer);
                    negativeImagesContent.Headers.ContentType =
                        MediaTypeHeaderValue.Parse("multipart/form-data");

                    var positiveFilename = Path.GetFileName(positiveExamplesPath);
                    var negativeFilename = Path.GetFileName(negativeExamplesPath);

                    request.Add(positiveImagesContent, "positive_examples", positiveFilename);
                    request.Add(negativeImagesContent, "negative_examples", negativeFilename);
                    request.Add(new StringContent(classifierName), "name");

                    var response = await client.PostAsync("api/v2/classifiers?version=" + VersionReleaseDate, request);
                    var msg = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase);
                    if (response.IsSuccessStatusCode)
                    {
                        var model = await response.Content.ReadAsAsync<Classifier>();
                        viewModel = ClassifierMapper.Map(model);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
            return viewModel;
        }

        public async Task<bool> DeleteClassifierAsync(string classifierId)
        {
            using (var client = VrClient())
            {
                try
                {
                    // Watson VR service will fail to delete the classifier if not converted to lowercase
                    var response = await client.DeleteAsync("api/v2/classifiers/" + classifierId + "?version=" + VersionReleaseDate);

                    var msg = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase);
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
            return false;
        }

        public async Task<bool> DeleteClassifierAsync(Classifier classifier)
        {
            return await DeleteClassifierAsync(classifier.ClassifierId);
        }

        public async Task<ClassifierViewModel> GetClassifierAsync(string classifierId)
        {
            ClassifierViewModel viewModel = null;
            using (var client = VrClient())
            {
                try
                {
                    var response = await client.GetAsync("api/v2/classifiers/" + classifierId + "?version=" + VersionReleaseDate);

                    var msg = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase);
                    if (response.IsSuccessStatusCode)
                    {
                        var model = await response.Content.ReadAsAsync<Classifier>();
                        viewModel = ClassifierMapper.Map(model);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
            return viewModel;
        }

        public async Task<ClassifiersResponse> GetClassifiersAsync()
        {
            ClassifiersResponse model = null;
            using (var client = VrClient())
            {
                try
                {
                    var response = await client.GetAsync("api/v2/classifiers?version=" + VersionReleaseDate);

                    var msg = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase);
                    if (response.IsSuccessStatusCode)
                    {
                        model = await response.Content.ReadAsAsync<ClassifiersResponse>();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
            return model;
        }

        private HttpClient VrClient()
        {
            var httpHandler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                UseCookies = true,
                CookieContainer = new CookieContainer()
            };

            var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(_vrCreds.Username + ":" + _vrCreds.Password));

            HttpClient client = HttpClientFactory.Create(httpHandler, new LoggingHandler());
            client.BaseAddress = new Uri(_vrCreds.Url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);

            if (learningOptOut)
            {
                client.DefaultRequestHeaders.Add("X-Watson-Learning-Opt-Out", learningOptOut.ToString());
            }

            return client;
        }
    }
}
