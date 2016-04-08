using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WatsonServices.Models.VisualRecognition;

namespace WatsonServices.Services
{
    public interface IVisualRecognitionService
    {
        bool ShareData { get; set; }

        Task<ClassifyResponse> ClassifyAsync(string filePath, params string[] classifierIds);
        Task<Classifier> CreateClassifierAsync(string positiveExamplesPath, string negativeExamplesPath, string classifierName);
        Task<bool> DeleteClassifierAsync(string classifierId);
        Task<bool> DeleteClassifierAsync(Classifier classifier);
        Task<Classifier> GetClassifierAsync(string classifierId);
        Task<ClassifiersResponse> GetClassifiersAsync();
    }

    public class VisualRecognitionService : IVisualRecognitionService
    {
        private bool learningOptOut;
        private readonly Credentials _vrCreds;

        // specifies which version of the Watson API to use
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

        public VisualRecognitionService(Credentials credentials)
        {
            _vrCreds = credentials;

            if (_vrCreds == null || _vrCreds.Username == null || _vrCreds.Password == null || _vrCreds.Url == null)
            {
                throw new Exception("Missing Watson VR service credentials");
            }
        }

        /// <summary>
        /// Classifies an image based on a given set of classifiers, or all classifiers if no classifiers are specified.
        /// </summary>
        /// <param name="filePath">Path to the image or zip file of images to be classified</param>
        /// <param name="maxScores">(Optional) Maximum number of classifier scores to list for each image</param>
        /// <param name="classifierIds">(Optional) Array of classifier Ids to use when classifying the image</param>
        /// <returns>A collection of images and their corresponding classifier scores</returns>
        public async Task<ClassifyResponse> ClassifyAsync(string filePath, params string[] classifierIds)
        {
            // read the file into a byte buffer and call ClassifyAsync overload with the byte buffer
            return await ClassifyAsync(Path.GetFileName(filePath), File.ReadAllBytes(filePath), classifierIds);
        }

        /// <summary>
        /// Classifies an image based on a given set of classifiers, or all classifiers if no classifiers are specified.
        /// </summary>
        /// <param name="filePath">Path to the image or zip file of images to be classified</param>
        /// <param name="maxScores">(Optional) Maximum number of classifier scores to list for each image</param>
        /// <param name="classifierIds">(Optional) Array of classifier Ids to use when classifying the image</param>
        /// <returns>A collection of images and their corresponding classifier scores</returns>
        public async Task<ClassifyResponse> ClassifyAsync(string filename, byte[] fileContents, params string[] classifierIds)
        {
            ClassifyResponse model = new ClassifyResponse();

            // Create an HttpClient to make the request using VrClient()
            using (var client = VrClient())
            {
                try
                {
                    // create a new MultipartFormDataContent to store the form data to be sent
                    var request = new MultipartFormDataContent();

                    // create a ByteArrayContent from the image file buffer and set its content type
                    var imageContent = new ByteArrayContent(fileContents);
                    imageContent.Headers.ContentType =
                        MediaTypeHeaderValue.Parse("multipart/form-data");

                    // if classifierIds was not omitted, serialize the array and add it to the request
                    if (classifierIds != null && classifierIds.Any() && classifierIds[0] != null)
                    {
                        var serializedClassifiers = "{\"classifier_ids\":" + 
                            Newtonsoft.Json.JsonConvert.SerializeObject(classifierIds) + "}";
                        request.Add(new StringContent(serializedClassifiers), "classifier_ids");
                    }
                    
                    // add the image content  to the form data
                    request.Add(imageContent, "images_file", filename);

                    // send a POST request to the Watson service with the form data from request
                    var response = await client.PostAsync("api/v2/classify?version=" + VersionReleaseDate, request);

                    // if the request succeeded, read the json result as a Response object and map it to the view model
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
        /// Creates a custom image classifier using a sets of positive and negative examples of the classifier.
        /// </summary>
        /// <param name="positiveExamplesZipFile">Path to a zip file containing positive examples</param>
        /// <param name="negativeExamplesZipFile">Path to a zip file containing negative examples</param>
        /// <param name="classifierName">Name of the new classifier</param>
        /// <returns>A <c>Classifier</c> representing the newly created classifier, or null if it could not be created</returns>
        public async Task<Classifier> CreateClassifierAsync(string positiveExamplesZipFile,
            string negativeExamplesZipFile, string classifierName)
        {
            Classifier model = null;

            // Create an HttpClient to make the request using VrClient()
            using (var client = VrClient())
            {
                try
                {
                    // read all bytes in the two zip files into a byte buffer for each file
                    byte[] positiveBuffer = File.ReadAllBytes(positiveExamplesZipFile);
                    byte[] negativeBuffer = File.ReadAllBytes(negativeExamplesZipFile);

                    // create a MultipartFormDataContent to store the form data to be sent
                    var request = new MultipartFormDataContent();

                    // create a ByteArrayContent from the positive examples buffer and set its content type
                    var positiveImagesContent = new ByteArrayContent(positiveBuffer);
                    positiveImagesContent.Headers.ContentType =
                        MediaTypeHeaderValue.Parse("multipart/form-data");

                    // create a ByteArrayContent from the negative examples buffer and set its content type
                    var negativeImagesContent = new ByteArrayContent(negativeBuffer);
                    negativeImagesContent.Headers.ContentType =
                        MediaTypeHeaderValue.Parse("multipart/form-data");

                    // get the filenames of the two zip files, without the path
                    var positiveFilename = Path.GetFileName(positiveExamplesZipFile);
                    var negativeFilename = Path.GetFileName(negativeExamplesZipFile);

                    // add each of the ByteArrayContents to the request and a StringContent with the name of the classifier
                    request.Add(positiveImagesContent, "positive_examples", positiveFilename);
                    request.Add(negativeImagesContent, "negative_examples", negativeFilename);
                    request.Add(new StringContent(classifierName), "name");

                    // send the request as a POST request to the Watson service
                    var response = await client.PostAsync("api/v2/classifiers?version=" + VersionReleaseDate, request);

                    // if the classifier was successfully created, the result code will be 200
                    // see https://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/visual-recognition/api/v2/#create_a_classifier
                    if (response.IsSuccessStatusCode)
                    {
                        // read the json result as a Classifier, then map it to a ClassifierViewModel
                        model = await response.Content.ReadAsAsync<Classifier>();
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
        /// <param name="classifier">A <c>Classifier</c> object representing the classifier to be deleted</param>
        /// <returns>boolean value indicating whether or not the delete operation was successful</returns>
        public async Task<bool> DeleteClassifierAsync(string classifierId)
        {
            // Create an HttpClient to make the request using VrClient()
            using (var client = VrClient())
            {
                try
                {
                    // send a DELETE request to the Watson service to delete the classifier with the specified Id
                    var response = await client.DeleteAsync("api/v2/classifiers/" + classifierId + "?version=" + VersionReleaseDate);

                    // if the classifier could not be found or could not be deleted, the response code will not be 200
                    // see https://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/visual-recognition/api/v2/#delete_a_classifier
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
                    // send a GET request to the Watson service to get the classifier which matches the specified classifier Id
                    var response = await client.GetAsync("api/v2/classifiers/" + classifierId + "?version=" + VersionReleaseDate);

                    // If the Watson service found a classifier with the specified Id, the HTTP status code will be 200
                    if (response.IsSuccessStatusCode)
                    {
                        // read the json result as a Classifier model, then map it to a ClassifierViewModel
                        model = await response.Content.ReadAsAsync<Classifier>();
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
        public async Task<ClassifiersResponse> GetClassifiersAsync()
        {
            ClassifiersResponse model = null;

            // Create an HttpClient to make the request using VrClient()
            using (var client = VrClient())
            {
                try
                {
                    // send a GET request to the Watson service to retrieve the list of available classifiers
                    var response = await client.GetAsync("api/v2/classifiers?version=" + VersionReleaseDate);

                    // the Watson service returns a 200 status code when the request was successful and
                    // a json object representing the list of classifiers
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

            // the Watson service uses basic authentication in the form of username:password, base64 encoded
            var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(_vrCreds.Username + ":" + _vrCreds.Password));

            HttpClient client = HttpClientFactory.Create(httpHandler, new LoggingHandler());
            // Set base address to the url provided from VCAP_SERVICES
            client.BaseAddress = new Uri(_vrCreds.Url);

            // Set request headers to accept json result and use basic authentication
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);

            // In order to opt out of sharing data with watson, the X-Watson-Learning-Opt-Out header must be set to true
            // See https://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/visual-recognition/api/v2/#data-collection
            if (learningOptOut)
            {
                client.DefaultRequestHeaders.Add("X-Watson-Learning-Opt-Out", learningOptOut.ToString());
            }

            return client;
        }
    }
}
