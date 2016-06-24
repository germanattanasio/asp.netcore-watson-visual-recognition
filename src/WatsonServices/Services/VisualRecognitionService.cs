using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WatsonServices.Models;
using WatsonServices.Models.VisualRecognition;

namespace WatsonServices.Services
{
    public interface IVisualRecognitionService : IWatsonLearningService, IWatsonFileService
    {
        Task<ClassifyResponse> ClassifyAsync(string url,
                                             AcceptLanguage acceptLanguage = AcceptLanguage.EN,
                                             double? threshold = null,
                                             ICollection<ClassifierOwner> owners = null,
                                             params string[] classifierIds);
        Task<ClassifyResponse> ClassifyAsync(string filename,
                                             byte[] fileContents,
                                             AcceptLanguage acceptLanguage = AcceptLanguage.EN,
                                             double? threshold = null,
                                             ICollection<ClassifierOwner> owners = null,
                                             params string[] classifierIds);
        Task<Classifier> CreateClassifierAsync(string classifierName,
                                               string negativeExamplesPath,
                                               params ClassPositiveExamples[] positiveExamples);
        Task<bool> DeleteClassifierAsync(string classifierId);
        Task<bool> DeleteClassifierAsync(Classifier classifier);
        Task<Classifier> GetClassifierAsync(string classifierId);
        Task<ClassifiersResponse> GetClassifiersAsync(bool verbose = false);
        Task<ClassifyResponse> GetImageSceneTextAsync(string url);
        Task<ClassifyResponse> GetImageSceneTextAsync(string imageFileName, byte[] imageFileContents);
        Task<ClassifyResponse> RecognizeFacesAsync(string url);
        Task<ClassifyResponse> RecognizeFacesAsync(string imageFilePath, string url = null);
        Task<ClassifyResponse> RecognizeFacesAsync(string imageFileName, byte[] imageFileContents, string url = null);
    }

    public class VisualRecognitionService : WatsonLearningService, IVisualRecognitionService
    {
        private readonly WatsonVisionCombinedCredentials _vrCreds;

        // specifies which version of the Watson API to use
        private const string VersionReleaseDate = "2016-05-20";
        // defines the User-Agent header that the service will send to the API
        private readonly static string UserAgent = typeof(VisualRecognitionService).Namespace.Split('.')[0] + "/" + 
                                                   typeof(VisualRecognitionService).GetTypeInfo().Assembly.GetName().Version;

        public VisualRecognitionService(WatsonVisionCombinedCredentials credentials)
        {
            if (credentials == null || !credentials.IsValid)
            {
                throw new Exception("Missing Watson Visual Recognition service credentials");
            }

            _vrCreds = credentials;
        }

        /// <summary>
        /// Classifies an image based on a given set of classifiers, or all classifiers if no classifiers are specified.
        /// </summary>
        /// <param name="url">The URL of an image (.jpg, or .png). Redirects are followed, so you can use shortened
        ///                   URLs. The resolved URL is returned in the response. Maximum image size is 2 MB.</param>
        /// <param name="acceptLanguage">(Optional) Specifies the language of the output. You can specify en for English,
        ///                              es for Spanish, ar for Arabic, or ja for Japanese. Classifiers for which no 
        ///                              translation is available are ommitted.  Default value is English.</param>
        /// <param name="threshold">(Optional) A floating value that specifies the minimum score a class must have to be
        ///                         displayed in the response. Setting the threshold to 0.0 will return all values, 
        ///                         regardless of their classification score.</param>
        /// <param name="owners">(Optional) A Collection with the value(s) ClassifierOwner.IBM and/or ClassifierOwner.Me
        ///                      to specify which classifiers to run.</param>
        /// <param name="classifierIds">(Optional) Array of classifier Ids to use when classifying the image</param>
        /// <returns>A collection of images and their corresponding classifier scores</returns>
        public async Task<ClassifyResponse> ClassifyAsync(string url,
            AcceptLanguage acceptLanguage = AcceptLanguage.EN,
            double? threshold = null,
            ICollection<ClassifierOwner> owners = null,
            params string[] classifierIds)
        {
            ClassifyResponse model = new ClassifyResponse();

            // Create an HttpClient to make the request using VrClient()
            using (var client = VrClient())
            {
                try
                {
                    var requestString = "api/v3/classify";

                    // add API key
                    requestString += "?api_key=" + _vrCreds.ApiKey;
                    // add API version
                    requestString += "&version=" + VersionReleaseDate;
                    // add URL
                    requestString += "&url=" + url;

                    // convert the classifierIds array to a comma-separated list and add it to the request
                    if (classifierIds?.Any() == true && classifierIds[0] != null)
                    {
                        requestString += "&classifier_ids=" + string.Join(",", classifierIds);
                    }

                    // convert the owners array to a comma-separated list and add it to the request
                    if (owners?.Any() == true)
                    {
                        requestString += "&owners=" + string.Join(",", owners.Select(m => m == ClassifierOwner.IBM ? m.ToString() : m.ToString().ToLowerInvariant()).ToList());
                    }

                    // if threshold was not omitted, add it to the request
                    if (threshold.HasValue)
                    {
                        requestString += "&threshold=" + threshold.Value.ToString("F2");
                    }

                    // set accepted languages in headers
                    client.DefaultRequestHeaders.AcceptLanguage.Clear();
                    client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(acceptLanguage.ToString().ToLowerInvariant()));

                    // send a GET request to the Watson service
                    var response = await client.GetAsync(requestString);

                    // if the request succeeded, read the json result as a Response object
                    if (response.IsSuccessStatusCode)
                    {
                        model = await response.Content.ReadAsAsync<ClassifyResponse>();
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
        /// Classifies an image based on a given set of classifiers, or all classifiers if no classifiers are specified.
        /// </summary>
        /// <param name="filename">The name of the image file to be classified</param>
        /// <param name="fileContents">A byte-array containing the contents of the image file to be classified</param>
        /// <param name="acceptLanguage">(Optional) Specifies the language of the output. You can specify en for English,
        ///                              es for Spanish, ar for Arabic, or ja for Japanese. Classifiers for which no 
        ///                              translation is available are ommitted.  Default value is English.</param>
        /// <param name="threshold">(Optional) A floating value that specifies the minimum score a class must have to be
        ///                         displayed in the response. Setting the threshold to 0.0 will return all values, 
        ///                         regardless of their classification score.</param>
        /// <param name="owners">(Optional) A Collection with the value(s) ClassifierOwner.IBM and/or ClassifierOwner.Me
        ///                      to specify which classifiers to run.</param>
        /// <param name="classifierIds">(Optional) Array of classifier Ids to use when classifying the image</param>
        /// <returns>A collection of images and their corresponding classifier scores</returns>
        public async Task<ClassifyResponse> ClassifyAsync(string filename,
            byte[] fileContents,
            AcceptLanguage acceptLanguage = AcceptLanguage.EN,
            double? threshold = null,
            ICollection<ClassifierOwner> owners = null,
            params string[] classifierIds)
        {
            ClassifyResponse model = new ClassifyResponse();

            // Create an HttpClient to make the request using VrClient()
            using (var client = VrClient())
            {
                try
                {
                    var parameters = new ClassifyParameters();

                    // if classifierIds was not omitted, convert the array to a comma-separated list and add it to the request
                    if (classifierIds != null && classifierIds.Any() && classifierIds[0] != null)
                    {
                        parameters.ClassifierIds = classifierIds;
                    }

                    // if owners was not omitted, convert the array to a comma-separated list and add it to the request
                    if (owners != null && owners.Any())
                    {
                        parameters.Owners = owners;
                    }

                    // if threshold was not omitted, add it to the request
                    if (threshold.HasValue)
                    {
                        parameters.Threshold = threshold.Value.ToString("F2");
                    }

                    // set accepted languages in headers
                    client.DefaultRequestHeaders.AcceptLanguage.Clear();
                    client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(acceptLanguage.ToString().ToLowerInvariant()));

                    // create request object
                    HttpContent imageContent = GetHttpContentFromImage(filename, fileContents);
                    MultipartFormDataContent request = CreateFileUploadRequest(GetHttpContentFromParameters(parameters), imageContent);

                    // send a POST request to the Watson service with the form data from request
                    var response = await client.PostAsync("api/v3/classify?api_key=" + _vrCreds.ApiKey + "&version=" + VersionReleaseDate, request);

                    // if the request succeeded, read the json result as a Response object
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = await response.Content.ReadAsStringAsync();
                        model = JsonConvert.DeserializeObject<ClassifyResponse>(jsonData);
                    }
                    else
                    {
                        var responseMessage = await response.Content.ReadAsStringAsync();
                        model.Error = new ErrorResponse()
                        {
                            Description = responseMessage
                        };
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
        /// Creates a custom image classifier using a sets of positive and negative examples of the classifier.
        /// </summary>
        /// <param name="positiveExamplesZipFile">Path to a zip file containing positive examples</param>
        /// <param name="negativeExamplesZipFile">Path to a zip file containing negative examples</param>
        /// <param name="classifierName">Name of the new classifier</param>
        /// <returns>A <c>Classifier</c> representing the newly created classifier, or null if it could not be created</returns>
        public async Task<Classifier> CreateClassifierAsync(string classifierName, string negativeExamplesZipFile, params ClassPositiveExamples[] positiveExamples)
        {
            Classifier model = null;

            // Create an HttpClient to make the request using VrClient()
            using (var client = VrClient())
            {
                try
                {
                    // populate FileContents for each ClassPositiveExamples given
                    foreach (var positiveExample in positiveExamples.Where(m => !(m.FileContents?.Any() == true) && !string.IsNullOrEmpty(m.FileName)).ToList())
                    {
                        positiveExample.FileContents = File.ReadAllBytes(positiveExample.FileName);
                    }

                    // read all bytes in the two zip files into a byte buffer for each file
                    byte[] negativeBuffer = null;
                    if (!string.IsNullOrEmpty(negativeExamplesZipFile))
                    {
                        negativeBuffer = File.ReadAllBytes(negativeExamplesZipFile);
                    }

                    // create a MultipartFormDataContent to store the form data to be sent
                    // create a list of HttpContent containing the positive examples
                    var httpContents = positiveExamples.Select(m => GetHttpContentFromPositiveExamples(m)).ToList();
                    // create an HttpContent from the negative examples buffer and add it to the list
                    if (negativeBuffer != null)
                    {
                        httpContents.Add(GetHttpContentFromNegativeExamples(Path.GetFileName(negativeExamplesZipFile), negativeBuffer));
                    }
                    // create the request object using the list
                    MultipartFormDataContent request = CreateFileUploadRequest(httpContents.ToArray());

                    request.Add(new StringContent(classifierName), "name");

                    // send the request as a POST request to the Watson service
                    var response = await client.PostAsync("api/v3/classifiers?api_key="+ _vrCreds.ApiKey + "&version=" + 
                        VersionReleaseDate, request);

                    // if the classifier was successfully created, the result code will be 200
                    // see https://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/visual-recognition/api/v3/#create_a_classifier
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = await response.Content.ReadAsStringAsync();
                        // deserialize the json result as a Classifier
                        model = JsonConvert.DeserializeObject<Classifier>(jsonData);
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
        /// Deletes a custom classifier
        /// </summary>
        /// <param name="classifierId">A <c>string</c> object representing the classifier id of the classifier to be deleted</param>
        /// <returns>boolean value indicating whether or not the delete operation was successful</returns>
        public async Task<bool> DeleteClassifierAsync(string classifierId)
        {
            // Create an HttpClient to make the request using VrClient()
            using (var client = VrClient())
            {
                try
                {
                    var requestString = "api/v3/classifiers/" + classifierId;

                    // add API key
                    requestString += "?api_key=" + _vrCreds.ApiKey;
                    // add API version
                    requestString += "&version=" + VersionReleaseDate;

                    // send a DELETE request to the Watson service to delete the classifier with the specified Id
                    var response = await client.DeleteAsync(requestString);

                    // if the classifier could not be found or could not be deleted, the response code will not be 200
                    // see https://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/visual-recognition/api/v3/#delete_a_classifier
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

        /// <summary>
        /// Deletes a custom classifier
        /// </summary>
        /// <param name="classifier">A <c>Classifier</c> object representing the classifier to be deleted</param>
        /// <returns>boolean value indicating whether or not the delete operation was successful</returns>
        public async Task<bool> DeleteClassifierAsync(Classifier classifier)
        {
            // call the DeleteClassifierAsync method using the specified classifier's Id
            return await DeleteClassifierAsync(classifier.ClassifierId);
        }

        /// <summary>
        /// Retrieves a specific classifier's information from the Watson service including name, id, owner, and created time
        /// </summary>
        /// <param name="classifierId">The Id string of the classifier to be retrieved</param>
        /// <returns>A <c>Classifier</c> representing the classifier, or null if the classifier was not found</returns>
        public async Task<Classifier> GetClassifierAsync(string classifierId)
        {
            Classifier model = null;

            // Create an HttpClient to make the request using VrClient()
            using (var client = VrClient())
            {
                try
                {
                    var requestString = "api/v3/classifiers/" + classifierId;

                    // add API key
                    requestString += "?api_key=" + _vrCreds.ApiKey;
                    // add API version
                    requestString += "&version=" + VersionReleaseDate;

                    // send a GET request to the Watson service to get the classifier which matches the specified classifier Id
                    var response = await client.GetAsync(requestString);

                    // If the Watson service found a classifier with the specified Id, the HTTP status code will be 200
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = await response.Content.ReadAsStringAsync();
                        // deserialize the json result as a Classifier model
                        model = JsonConvert.DeserializeObject<Classifier>(jsonData);
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
        /// Retrieves a list of available classifiers from the Watson service.
        /// </summary>
        /// <returns>A <c>ClassifiersResponse</c> object which contains a list of all available classifiers</returns>
        public async Task<ClassifiersResponse> GetClassifiersAsync(bool verbose = false)
        {
            ClassifiersResponse model = null;

            // Create an HttpClient to make the request using VrClient()
            using (var client = VrClient())
            {
                try
                {
                    // send a GET request to the Watson service to retrieve the list of available classifiers
                    var response = await client.GetAsync("api/v3/classifiers?api_key=" + _vrCreds.ApiKey + "&version=" +
                        VersionReleaseDate + "&verbose=" + verbose.ToString());

                    // the Watson service returns a 200 status code when the request was successful and
                    // a json object representing the list of classifiers
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = await response.Content.ReadAsStringAsync();
                        model = JsonConvert.DeserializeObject<ClassifiersResponse>(jsonData);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
            return model;
        }

        public async Task<ClassifyResponse> GetImageSceneTextAsync(string url)
        {
            ClassifyResponse model = new ClassifyResponse();

            // Create an HttpClient to make the request using VrClient()
            using (var client = VrClient())
            {
                try
                {
                    var requestString = "api/v3/recognize_text";

                    // add API key
                    requestString += "?api_key=" + _vrCreds.ApiKey;
                    // add API version
                    requestString += "&version=" + VersionReleaseDate;
                    // add url
                    requestString += "&url=" + url;

                    // send a GET request to the Visual Recognition service
                    var response = await client.GetAsync(requestString);

                    // if the request succeeded, read the json result as a Response object
                    if (response.IsSuccessStatusCode)
                    {
                        model = await response.Content.ReadAsAsync<ClassifyResponse>();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return model;
        }

        public async Task<ClassifyResponse> GetImageSceneTextAsync(string imageFileName, byte[] imageFileContents)
        {
            ClassifyResponse model = new ClassifyResponse();

            // Create an HttpClient to make the request using VrClient()
            using (var client = VrClient())
            {
                try
                {
                    var requestString = "api/v3/recognize_text";

                    // add API key
                    requestString += "?api_key=" + _vrCreds.ApiKey;
                    // add API version
                    requestString += "&version=" + VersionReleaseDate;

                    HttpContent imageContent = GetHttpContentFromImage(imageFileName, imageFileContents);
                    var request = CreateFileUploadRequest(imageContent);

                    // send a POST request to the AlchemyAPI service
                    var response = await client.PostAsync(requestString, request);

                    // if the request succeeded, read the json result as an ClassifyResponse object
                    if (response.IsSuccessStatusCode)
                    {
                        model = await response.Content.ReadAsAsync<ClassifyResponse>();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return model;
        }

        public async Task<ClassifyResponse> RecognizeFacesAsync(string url)
        {
            ClassifyResponse model = new ClassifyResponse();

            // Create an HttpClient to make the request using VrClient()
            using (var client = VrClient())
            {
                try
                {
                    var requestString = "api/v3/detect_faces";

                    // add API key
                    requestString += "?api_key=" + _vrCreds.ApiKey;
                    // add API version
                    requestString += "&version=" + VersionReleaseDate;
                    // add url
                    requestString += "&url=" + url;

                    // send a GET request to the AlchemyAPI service
                    var response = await client.GetAsync(requestString);

                    // if the request succeeded, read the json result as a Response object
                    if (response.IsSuccessStatusCode)
                    {
                        model = await response.Content.ReadAsAsync<ClassifyResponse>();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return model;
        }

        public async Task<ClassifyResponse> RecognizeFacesAsync(string imageFilePath, string url = null)
        {
            return await RecognizeFacesAsync(Path.GetFileName(imageFilePath), File.ReadAllBytes(imageFilePath), url);
        }

        public async Task<ClassifyResponse> RecognizeFacesAsync(string imageFileName, byte[] imageFileContents, string url = null)
        {
            ClassifyResponse model = new ClassifyResponse();

            // Create an HttpClient to make the request using VrClient()
            using (var client = VrClient())
            {
                try
                {
                    var requestString = "api/v3/detect_faces";

                    // add API key
                    requestString += "?api_key=" + _vrCreds.ApiKey;
                    // add API version
                    requestString += "&version=" + VersionReleaseDate;

                    // add url to request parameters
                    ClassifyParameters parameters = new ClassifyParameters() { Url = url };

                    HttpContent imageContent = GetHttpContentFromImage(imageFileName, imageFileContents);
                    var request = CreateFileUploadRequest(GetHttpContentFromParameters(parameters), imageContent);

                    // send a POST request to the Visual Recognition service
                    var response = await client.PostAsync(requestString, request);

                    // if the request succeeded, read the json result as a ClassifyResponse object
                    if (response.IsSuccessStatusCode)
                    {
                        model = await response.Content.ReadAsAsync<ClassifyResponse>();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return model;
        }

        private static MultipartFormDataContent CreateFileUploadRequest(params HttpContent[] uploadContents)
        {
            var guid = Guid.NewGuid().ToString().Split('-');
            var boundary = "-----------------------" + guid[3] + guid[4];
            // create a new MultipartFormDataContent to store the form data to be sent
            var request = new MultipartFormDataContent(boundary);

            if (uploadContents != null)
            {
                foreach (var uploadContent in uploadContents.Where(m => m != null).ToList())
                {
                    // add each file upload to the form data
                    request.Add(uploadContent);
                }
            }

            request.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data; boundary=" + boundary);
            return request;
        }

        private static HttpContent GetHttpContentFromImage(string filename, byte[] fileContents)
        {
            // create a ByteArrayContent from the image file buffer and set its content type
            var imageContent = new ByteArrayContent(fileContents);
            imageContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
            imageContent.Headers.ContentDisposition.Name = "\"images_file\"";
            imageContent.Headers.ContentDisposition.FileName = "\"" + filename + "\"";
            var fileExt = Path.GetExtension(filename).ToLowerInvariant();
            switch(fileExt)
            {
                case ".jpg":
                case ".jpeg":
                    imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                    break;
                case ".png":
                    imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
                    break;
                case ".zip":
                    imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/zip");
                    break;
                default:
                    break;
            }
            return imageContent;
        }

        private static HttpContent GetHttpContentFromNegativeExamples(string filename, byte[] fileContents)
        {
            // create a ByteArrayContent from the image file buffer and set its content type
            var content = new ByteArrayContent(fileContents);
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
            content.Headers.ContentDisposition.Name = "\"negative_examples\"";
            content.Headers.ContentDisposition.FileName = "\"" + filename + "\"";
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/zip");
            return content;
        }

        private static HttpContent GetHttpContentFromParameters(ClassifyParameters parameters)
        {
            if (parameters?.HasContent != true)
                return null;

            var parametersString = Newtonsoft.Json.JsonConvert.SerializeObject(parameters);
            var parametersContent = new ByteArrayContent(Encoding.ASCII.GetBytes(parametersString));
            parametersContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
            parametersContent.Headers.ContentDisposition.Name = "\"parameters\"";
            parametersContent.Headers.ContentDisposition.FileName = "\"myparams.json\"";
            parametersContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
            return parametersContent;
        }

        private static HttpContent GetHttpContentFromPositiveExamples(ClassPositiveExamples positiveExamples)
        {
            if (string.IsNullOrEmpty(positiveExamples?.ClassName) || string.IsNullOrEmpty(positiveExamples?.FileName) ||
                positiveExamples?.FileContents?.Any() != true)
                return null;

            // create a ByteArrayContent from the image file buffer and set its content type
            var content = new ByteArrayContent(positiveExamples.FileContents);
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
            // replace all whitespace characters in class name with underscores
            content.Headers.ContentDisposition.Name = "\"" + Regex.Replace(positiveExamples.ClassName, @"\s+", "_") + "_positive_examples\"";
            content.Headers.ContentDisposition.FileName = "\"" + positiveExamples.FileName + "\"";
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/zip");
            return content;
        }

        /// <summary>
        /// This method provides a pre-configured <c>HttpClient</c> for connecting to the Watson REST API.
        /// </summary>
        /// <returns>An <c>HttpClient</c> which is pre-configured for connecting to the Watson REST API</returns>
        private HttpClient VrClient()
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
            client.BaseAddress = new Uri(_vrCreds.ApiEndPoint);

            // Set request headers to accept json result
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.ExpectContinue = true;
            client.DefaultRequestHeaders.Connection.Clear();
            client.DefaultRequestHeaders.UserAgent.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", UserAgent);

            // In order to opt out of sharing data with Watson, the X-Watson-Learning-Opt-Out header must be set to true
            // See https://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/visual-recognition/api/v3/#data-collection
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
                case ".jpg":
                case ".jpeg":
                case ".zip":
                case ".png":
                    return true;
                default:
                    return false;
            }
        }
    }
}
