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

        public VisualRecognitionService(IOptions<Credentials> optionsAccessor)
        {
            // retrieve Watson service credentials from Configuration (set in Startup.cs)
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

            // set accepted image types, the Watson service officially supports jpg, jpeg, and png
            // other image types could be converted to one of these formats before sending to Watson
            _acceptedImageTypes = new HashSet<string>()
            {
                ".jpg",
                ".jpeg",
                ".png"
            };
        }

        /// <summary>
        /// Classifies an image based on a given set of classifiers, or all classifiers if no classifiers are specified.
        /// </summary>
        /// <param name="filePath">Path to the image or zip file of images to be classified</param>
        /// <param name="maxScores">(Optional) Maximum number of classifier scores to list for each image</param>
        /// <param name="classifierIds">(Optional) Array of classifier Ids to use when classifying the image</param>
        /// <returns>A collection of images and their corresponding classifier scores</returns>
        public async Task<VisualRecognitionViewModel> ClassifyAsync(string filePath, int? maxScores = null, params string[] classifierIds)
        {
            VisualRecognitionViewModel viewModel = new VisualRecognitionViewModel();

            // Create an HttpClient to make the request using VrClient()
            using (var client = VrClient())
            {
                try
                {
                    // read the file into a byte buffer
                    byte[] buffer = File.ReadAllBytes(filePath);

                    // create a new MultipartFormDataContent to store the form data to be sent
                    var request = new MultipartFormDataContent();

                    // create a ByteArrayContent from the image file buffer and set its content type
                    var imageContent = new ByteArrayContent(buffer);
                    imageContent.Headers.ContentType =
                        MediaTypeHeaderValue.Parse("multipart/form-data");

                    // get the filename of the image file, without the path
                    var filename = Path.GetFileName(filePath);

                    // create a dictionary to store the original filenames and the base64 encoded image files (for displaying images)
                    var base64Images = new Dictionary<string, string>();
                    Task extractImages = null;

                    // if the file is not a zip file, add the filename and it's base64 encoded form to base64Images
                    if (Path.GetExtension(filePath).ToLowerInvariant() != ".zip")
                    {
                        base64Images.Add(filename, Convert.ToBase64String(File.ReadAllBytes(filePath)));
                    }
                    else
                    {
                        // create and run a new task to extract the files from the zip file in the background
                        extractImages = new TaskFactory().StartNew(() =>
                        {
                            var fileName = Path.GetFileNameWithoutExtension(filePath);
                            var folder = Path.GetDirectoryName(filePath);
                            var newTempFolder = Path.Combine(folder, fileName);
                            System.IO.Compression.ZipFile.ExtractToDirectory(filePath, newTempFolder);

                            // add each of the extracted images to the base64Images dictionary
                            foreach (var file in Directory.GetFiles(newTempFolder)
                                .Where(m => _acceptedImageTypes.Contains(Path.GetExtension(m).ToLowerInvariant())).ToList())
                            {
                                base64Images.Add(Path.GetFileName(file), Convert.ToBase64String(File.ReadAllBytes(file)));
                            }

                            // cleanup the extracted files as they are no longer needed
                            foreach (var file in Directory.GetFiles(newTempFolder))
                            {
                                File.Delete(file);
                            }
                            Directory.Delete(newTempFolder);
                        });
                    }

                    // if classifierIds was not omitted, serialize the array and add it to the request
                    if (classifierIds != null)
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
                        var model = await response.Content.ReadAsAsync<Response>();
                        if (extractImages != null)
                        {
                            // wait for the image extraction to finish if the uploaded file was a zip file
                            Task.WaitAll(extractImages);
                        }

                        // use the ImagesMapper to map the response from Watson to the base64 images and original filenames
                        ImagesMapper.Map(model, viewModel, base64Images, maxScores);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
            return viewModel;
        }

        /// <summary>
        /// Creates a custom image classifier using a sets of positive and negative examples of the classifier.
        /// </summary>
        /// <param name="positiveExamplesZipFile">Path to a zip file containing positive examples</param>
        /// <param name="negativeExamplesZipFile">Path to a zip file containing negative examples</param>
        /// <param name="classifierName">Name of the new classifier</param>
        /// <returns>A <c>ClassifierViewModel</c> representing the newly created classifier, or null if it could not be created</returns>
        public async Task<ClassifierViewModel> CreateClassifierAsync(string positiveExamplesZipFile,
            string negativeExamplesZipFile, string classifierName)
        {
            ClassifierViewModel viewModel = null;

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
        /// <returns>A <c>ClassifierViewModel</c> representing the classifier, or null if the classifier was not found</returns>
        public async Task<ClassifierViewModel> GetClassifierAsync(string classifierId)
        {
            ClassifierViewModel viewModel = null;

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
