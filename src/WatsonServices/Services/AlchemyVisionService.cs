using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WatsonServices.Models;
using WatsonServices.Models.AlchemyVision;

namespace WatsonServices.Services
{
    public interface IAlchemyVisionService : IWatsonLearningService, IWatsonFileService
    {
        Task<ImageKeywordResponse> GetImageKeywordsAsync(string url, double forceShowAllParameter, bool knowledgeGraph);
        Task<ImageKeywordResponse> GetImageKeywordsAsync(byte[] fileContents, bool knowledgeGraph, string url = "");
        Task<ImageKeywordResponse> GetImageKeywordsAsync(Stream fileContentStream, bool knowledgeGraph, string url = "");
        Task<ImageKeywordResponse> GetImageKeywordsAsync(string imageFilePath, bool knowledgeGraph, string url = "");

        Task<ImageLinkResponse> GetImageLinksAsync(string url);
        Task<ImageLinkResponse> GetImageLinksAsync(string html, string url);

        Task<ImageSceneTextResponse> GetImageSceneTextAsync(string image, ImagePostType type);

        Task<FacialRecognitionResponse> RecognizeFacesAsync(string url, bool knowledgeGraph);
        Task<FacialRecognitionResponse> RecognizeFacesAsync(string imageFilePath, bool knowledgeGraph, string url = "");
    }

    public class AlchemyVisionService : WatsonLearningService, IAlchemyVisionService
    {
        private readonly AlchemyAPICredentials _apiCreds;

        public AlchemyVisionService(AlchemyAPICredentials apiCreds)
        {
            if (apiCreds == null || !apiCreds.IsValid)
            {
                throw new ArgumentNullException("apiCreds", "Must specify both the API key and the API endpoint.");
            }

            _apiCreds = apiCreds;
        }

        public async Task<ImageKeywordResponse> GetImageKeywordsAsync(string url, double forceShowAllParameter, bool knowledgeGraph)
        {
            ImageKeywordResponse model = new ImageKeywordResponse();

            // Create an HttpClient to make the request using WatsonJsonClient()
            using (var client = WatsonJsonClient())
            {
                try
                {
                    var requestString = "url/URLGetRankedImageKeywords";
                    // add url
                    requestString += "?url=" + url;
                    // add API key
                    requestString += "&apikey=" + _apiCreds.ApiKey;
                    // add output mode JSON
                    requestString += "&outputMode=json";
                    // add forceShowAllparameter
                    requestString += "&forceShowAllparameter=" + forceShowAllParameter;
                    // add knowledgeGraph
                    requestString += "&knowledgeGraph=" + (knowledgeGraph ? 1 : 0);

                    // send a GET request to the AlchemyAPI service
                    var response = await client.GetAsync(requestString);

                    // if the request succeeded, read the json result as a Response object
                    if (response.IsSuccessStatusCode)
                    {
                        model = await response.Content.ReadAsAsync<ImageKeywordResponse>();
                    }

                    model.HttpStatusCode = response.StatusCode;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return model;
        }

        public async Task<ImageKeywordResponse> GetImageKeywordsAsync(string imageFilePath, bool knowledgeGraph, string url = "")
        {
            byte[] fileBuffer;
            try
            {
                fileBuffer = File.ReadAllBytes(imageFilePath);
            }
            catch (Exception)
            {
                fileBuffer = new byte[0];
            }
            return await GetImageKeywordsAsync(fileBuffer, knowledgeGraph, url);
        }

        public async Task<ImageKeywordResponse> GetImageKeywordsAsync(byte[] fileContents, bool knowledgeGraph, string url = "")
        {
            ImageKeywordResponse result = null;

            using (var stream = new MemoryStream(fileContents))
            {
                result = await GetImageKeywordsAsync(stream, knowledgeGraph, url);
            }

            return result;
        }

        public async Task<ImageKeywordResponse> GetImageKeywordsAsync(Stream fileContentStream, bool knowledgeGraph, string url = "")
        {
            ImageKeywordResponse model = new ImageKeywordResponse();

            // Create an HttpClient to make the request using WatsonJsonClient()
            using (var client = WatsonJsonClient())
            {
                try
                {
                    var requestString = "image/ImageGetRankedImageKeywords";

                    // add image post mode
                    requestString += "?imagePostMode=raw";
                    // add API key
                    requestString += "&apikey=" + _apiCreds.ApiKey;
                    // add output mode JSON
                    requestString += "&outputMode=json";
                    // add knowledgeGraph
                    requestString += "&knowledgeGraph=" + (knowledgeGraph ? 1 : 0);
                    // add url
                    if (!string.IsNullOrEmpty(url))
                    {
                        requestString += "&url=" + url;
                    }

                    var request = new StreamContent(fileContentStream);

                    // send a POST request to the AlchemyAPI service
                    var response = await client.PostAsync(requestString, request);

                    // if the request succeeded, read the json result as a Response object
                    if (response.IsSuccessStatusCode)
                    {
                        model = await response.Content.ReadAsAsync<ImageKeywordResponse>();
                    }

                    model.HttpStatusCode = response.StatusCode;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return model;
        }

        public async Task<ImageLinkResponse> GetImageLinksAsync(string url)
        {
            ImageLinkResponse model = new ImageLinkResponse();

            // Create an HttpClient to make the request using WatsonJsonClient()
            using (var client = WatsonJsonClient())
            {
                try
                {
                    var requestString = "url/URLGetImage";

                    // add url
                    requestString += "?url=" + url;
                    // add API key
                    requestString += "&apikey=" + _apiCreds.ApiKey;
                    // add output mode JSON
                    requestString += "&outputMode=json";

                    // send a GET request to the AlchemyAPI service
                    var response = await client.GetAsync(requestString);

                    // if the request succeeded, read the json result as a Response object
                    if (response.IsSuccessStatusCode)
                    {
                        model = await response.Content.ReadAsAsync<ImageLinkResponse>();
                    }

                    model.HttpStatusCode = response.StatusCode;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return model;
        }

        public async Task<ImageLinkResponse> GetImageLinksAsync(string html, string url)
        {
            ImageLinkResponse model = new ImageLinkResponse();

            // Create an HttpClient to make the request using WatsonJsonClient()
            using (var client = WatsonJsonClient())
            {
                try
                {
                    var requestString = "html/HTMLGetImage";

                    // add API key
                    requestString += "?apikey=" + _apiCreds.ApiKey;
                    // add output mode JSON
                    requestString += "&outputMode=json";
                    // add url
                    requestString += "&url=" + url;

                    var request = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("html", html)
                    });

                    // send a POST request to the AlchemyAPI service
                    var response = await client.PostAsync(requestString, request);

                    // if the request succeeded, read the json result as a Response object
                    if (response.IsSuccessStatusCode)
                    {
                        model = await response.Content.ReadAsAsync<ImageLinkResponse>();
                    }

                    model.HttpStatusCode = response.StatusCode;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return model;
        }

        public async Task<ImageSceneTextResponse> GetImageSceneTextAsync(string image, ImagePostType type)
        {
            if (type == ImagePostType.Url)
            {
                return await GetImageSceneTextFromUrlAsync(image);
            }
            else if (type == ImagePostType.File)
            {
                return await GetImageSceneTextFromFileAsync(image);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private async Task<ImageSceneTextResponse> GetImageSceneTextFromUrlAsync(string url)
        {
            ImageSceneTextResponse model = new ImageSceneTextResponse();

            // Create an HttpClient to make the request using WatsonJsonClient()
            using (var client = WatsonJsonClient())
            {
                try
                {
                    var requestString = "url/URLGetRankedImageSceneText";

                    // add url
                    requestString += "?url=" + url;
                    // add API key
                    requestString += "&apikey=" + _apiCreds.ApiKey;
                    // add output mode JSON
                    requestString += "&outputMode=json";

                    // send a GET request to the AlchemyAPI service
                    var response = await client.GetAsync(requestString);

                    // if the request succeeded, read the json result as a Response object
                    if (response.IsSuccessStatusCode)
                    {
                        model = await response.Content.ReadAsAsync<ImageSceneTextResponse>();
                    }

                    model.HttpStatusCode = response.StatusCode;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return model;
        }

        private async Task<ImageSceneTextResponse> GetImageSceneTextFromFileAsync(string imageFile)
        {
            ImageSceneTextResponse model = new ImageSceneTextResponse();

            // Create an HttpClient to make the request using WatsonJsonClient()
            using (var client = WatsonJsonClient())
            {
                try
                {
                    byte[] fileBytes = File.ReadAllBytes(imageFile);
                    using (var fileStream = new MemoryStream(fileBytes))
                    {
                        var requestString = "image/ImageGetRankedImageSceneText";

                        // add image post mode
                        requestString += "?imagePostMode=raw";
                        // add API key
                        requestString += "&apikey=" + _apiCreds.ApiKey;
                        // add output mode JSON
                        requestString += "&outputMode=json";

                        var request = new StreamContent(fileStream);

                        // send a POST request to the AlchemyAPI service
                        var response = await client.PostAsync(requestString, request);

                        // if the request succeeded, read the json result as an ImageSceneTextResponse object
                        if (response.IsSuccessStatusCode)
                        {
                            model = await response.Content.ReadAsAsync<ImageSceneTextResponse>();
                        }

                        model.HttpStatusCode = response.StatusCode;

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return model;
        }

        public async Task<FacialRecognitionResponse> RecognizeFacesAsync(string url, bool knowledgeGraph)
        {
            FacialRecognitionResponse model = new FacialRecognitionResponse();

            // Create an HttpClient to make the request using WatsonJsonClient()
            using (var client = WatsonJsonClient())
            {
                try
                {
                    var requestString = "url/URLGetRankedImageFaceTags";

                    // add url
                    requestString += "?url=" + url;
                    // add API key
                    requestString += "&apikey=" + _apiCreds.ApiKey;
                    // add output mode JSON
                    requestString += "&outputMode=json";
                    // add knowledgeGraph
                    requestString += "&knowledgeGraph=" + (knowledgeGraph ? 1 : 0);

                    // send a GET request to the AlchemyAPI service
                    var response = await client.GetAsync(requestString);

                    // if the request succeeded, read the json result as a Response object
                    if (response.IsSuccessStatusCode)
                    {
                        model = await response.Content.ReadAsAsync<FacialRecognitionResponse>();
                    }

                    model.HttpStatusCode = response.StatusCode;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return model;
        }

        public async Task<FacialRecognitionResponse> RecognizeFacesAsync(string imageFilePath, bool knowledgeGraph, string url = "")
        {
            FacialRecognitionResponse model = new FacialRecognitionResponse();

            // Create an HttpClient to make the request using WatsonJsonClient()
            using (var client = WatsonJsonClient())
            {
                try
                {
                    byte[] fileBytes = File.ReadAllBytes(imageFilePath);
                    using (var fileStream = new MemoryStream(fileBytes))
                    {
                        var requestString = "image/ImageGetRankedImageFaceTags";

                        // add image post mode
                        requestString += "?imagePostMode=raw";
                        // add API key
                        requestString += "&apikey=" + _apiCreds.ApiKey;
                        // add output mode JSON
                        requestString += "&outputMode=json";
                        // add knowledgeGraph
                        requestString += "&knowledgeGraph=" + (knowledgeGraph ? 1 : 0);
                        // add url, if it's not null
                        if (!string.IsNullOrEmpty(url))
                        {
                            requestString += "&url=" + url;
                        }

                        var request = new StreamContent(fileStream);

                        // send a POST request to the AlchemyAPI service
                        var response = await client.PostAsync(requestString, request);

                        // if the request succeeded, read the json result as a FacialRecognitionResponse object
                        if (response.IsSuccessStatusCode)
                        {
                            model = await response.Content.ReadAsAsync<FacialRecognitionResponse>();
                        }

                        model.HttpStatusCode = response.StatusCode;

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return model;
        }

        /// <summary>
        /// This method provides a pre-configured <c>HttpClient</c> for connecting to the Watson REST API.
        /// This client will only accept application/json results.
        /// </summary>
        /// <returns>An <c>HttpClient</c> which is pre-configured for connecting to the Watson REST API</returns>
        private HttpClient WatsonJsonClient()
        {
            // Set http handler to use gzip and deflate for automatic decompression and accept cookies
            var httpHandler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                UseCookies = true,
                CookieContainer = new CookieContainer()
            };

            HttpClient client = HttpClientFactory.Create(httpHandler, new LoggingHandler());
            // Set base address to the url provided from VCAP_SERVICES
            // if the url doesn't end in /, the last part isn't used as part of the BaseAddress
            client.BaseAddress = new Uri(_apiCreds.ApiEndPoint + '/');

            // Set request headers to accept json result and use basic authentication
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // In order to opt out of sharing data with Watson, the X-Watson-Learning-Opt-Out header must be set to true
            // See http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/alchemyvision/api/v1/#data-collection
            if (learningOptOut)
            {
                client.DefaultRequestHeaders.Add("X-Watson-Learning-Opt-Out", learningOptOut.ToString());
            }

            return client;
        }

        bool IWatsonFileService.SupportsFileExtension(string fileExt)
        {
            var lowerFileExt = fileExt.ToLowerInvariant();
            switch (lowerFileExt)
            {
                case ".gif":
                case ".jpg":
                case ".jpeg":
                case ".png":
                    return true;
                default:
                    return false;
            }
        }
    }
}
