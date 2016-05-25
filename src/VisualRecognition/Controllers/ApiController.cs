using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.IO;
using System.Threading.Tasks;
using VisualRecognition.Mappers;
using VisualRecognition.Services;
using VisualRecognition.ViewModels;
using WatsonServices.Services;

namespace VisualRecognition.Controllers
{
    public class ApiController : Controller
    {
        private readonly IAlchemyVisionService _alchemyVisionService;
        private readonly IFileEncoderService _fileEncoderService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IVisualRecognitionService _visualRecognitionService;

        public ApiController(IAlchemyVisionService alchemyVisionService,
            IFileEncoderService fileEncoderService,
            IHostingEnvironment hostingEnvironment,
            IVisualRecognitionService visualRecognitionService)
        {
            _alchemyVisionService = alchemyVisionService;
            _fileEncoderService = fileEncoderService;
            _hostingEnvironment = hostingEnvironment;
            _visualRecognitionService = visualRecognitionService;

            // set to false, or comment these lines to opt out of sharing data with Watson
            _alchemyVisionService.ShareData = true;
            _visualRecognitionService.ShareData = true;
        }

        // GET: api/classifiers
        public async Task<IActionResult> Classifiers([FromBody]CreateClassifierViewModel viewModel)
        {
            if (viewModel == null)
            {
                return BadRequest("Missing view model");
            }

            // check if positive images are missing, return 400 if so
            if (viewModel.Positives == null)
            {
                return BadRequest("Missing positive images");
            }

            // check if negative images are missing, return 400 if so
            if (viewModel.Negatives == null)
            {
                return BadRequest("Missing negative images");
            }

            // check size of positive images array
            if (viewModel.Positives.Length < 10)
            {
                return BadRequest("Minimum positive images (10) sent:" + viewModel.Positives.Length);
            }

            // check size of positive images array
            if (viewModel.Negatives.Length < 10)
            {
                return BadRequest("Minimum negative images (10) sent:" + viewModel.Negatives.Length);
            }

            // either the images are base64 encoded images, or they're relative paths for included datasets
            bool base64 = viewModel.Positives[0]?.Split(new string[] { "base64" }, 2, StringSplitOptions.RemoveEmptyEntries)?.Length == 2;

            string positiveFolder = "";
            string negativeFolder = "";
            string positiveZipFile = "";
            string negativeZipFile = "";

            // create a zip file with uploaded files
            if (base64)
            {
                var uploadFolder = CreateTempUploadDirectory();
                positiveFolder = Path.Combine(uploadFolder, "positives");
                negativeFolder = Path.Combine(uploadFolder, "negatives");
                Directory.CreateDirectory(positiveFolder);
                Directory.CreateDirectory(negativeFolder);
                Task[] PositiveTasks = new Task[viewModel.Positives.Length];
                Task[] NegativeTasks = new Task[viewModel.Negatives.Length];
                for (int i = 0; i < viewModel.Positives.Length; i++)
                {
                    int x = i;
                    PositiveTasks[i] = new Task(
                        async () => await SaveUploadedFile(viewModel.Positives[x], positiveFolder, x.ToString())
                        );
                    PositiveTasks[i].Start();
                }
                for (int i = 0; i < viewModel.Negatives.Length; i++)
                {
                    int x = i;
                    NegativeTasks[i] = new Task(
                        async () => await SaveUploadedFile(viewModel.Negatives[x], negativeFolder, x.ToString())
                        );
                    NegativeTasks[i].Start();
                }
                Task.WaitAll(PositiveTasks);
                Task.WaitAll(NegativeTasks);
            }
            else
            {
                positiveFolder = Path.Combine(_hostingEnvironment.WebRootPath, Path.GetDirectoryName(viewModel.Positives[0]));
                negativeFolder = Path.Combine(_hostingEnvironment.WebRootPath, Path.GetDirectoryName(viewModel.Negatives[0]));
            }

            positiveZipFile = positiveFolder + ".zip";
            negativeZipFile = negativeFolder + ".zip";
            System.IO.Compression.ZipFile.CreateFromDirectory(positiveFolder, positiveZipFile);
            System.IO.Compression.ZipFile.CreateFromDirectory(negativeFolder, negativeZipFile);

            // call VisualRecognitionService and return the created classifier
            var result = new JsonResult(await _visualRecognitionService.CreateClassifierAsync(positiveZipFile, negativeZipFile, viewModel.Name));

            // cleanup can happen after the result is returned to the front-end
            // disable the warning about this task continuing after the method ends
#pragma warning disable 4014
            Task.Run(() =>
            {
                if (base64)
                {
                    // recursively delete all files and folders under and including the upload folder
                    Directory.Delete(Path.Combine(positiveFolder, ".."), true);
                }
                else
                {
                    // directory containing these files was not a temporary directory
                    System.IO.File.Delete(positiveZipFile);
                    System.IO.File.Delete(negativeZipFile);
                }

                // delete the classifier after 1 hour
                if (((WatsonServices.Models.VisualRecognition.Classifier)result?.Value)?.ClassifierId != null)
                {
                    var classifierId = ((WatsonServices.Models.VisualRecognition.Classifier)result.Value).ClassifierId;
                    Task.Factory.StartNew(() =>
                    {
                        Task.Delay(new TimeSpan(1, 0, 0)).ContinueWith(async (task) =>
                        {
                            await _visualRecognitionService
                                .DeleteClassifierAsync(classifierId);
                        });
                    });
                }
            });
#pragma warning restore 4014

            result.StatusCode = 200;
            return result;
        }

        // POST api/classify/
        [HttpPost]
        public async Task<IActionResult> Classify([ModelBinder(BinderType = typeof(ClassifyImageViewModelBinder))]ClassifyImageViewModel viewModel)
        {
            if (viewModel == null)
            {
                return new BadRequestResult();
            }

            if (viewModel.ImageByteContent?.Length > 0)
            {
                if (Request.Query.Keys.Contains("classifier_id"))
                {
                    return new JsonResult(
                        ImagesMapper.Map(await _visualRecognitionService.ClassifyAsync(
                            viewModel.ImageFileName, viewModel.ImageByteContent, viewModel.Classifier.ClassifierId))
                    );
                }

                return new JsonResult(
                    ImagesMapper.Map(await _alchemyVisionService.GetImageKeywordsAsync(
                        viewModel.ImageByteContent, false, viewModel.Url))
                );
            }

            // byte content was null or 0 length
            return new BadRequestResult();
        }

        private async Task SaveUploadedFile(string uploadedFile, string uploadPath, string filename)
        {
            string fileExt = "";

            string[] encodedFileParts = uploadedFile.Split(';');
            string encodedFile = encodedFileParts[1].Split(',')[1];
            var fileType = encodedFileParts[0].Split('/')[1];
            fileExt = "." + fileType.ToLowerInvariant();

            using (FileStream fs = new FileStream(Path.Combine(uploadPath, filename + fileExt), FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var byteArray = await _fileEncoderService.DecodeFileAsync(encodedFile);
                await fs.WriteAsync(byteArray, 0, byteArray.Length);
            }
        }

        private string CreateTempUploadDirectory()
        {
            string filePath = "";
            while (Directory.Exists(
                filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "temp_files", "uploads", Guid.NewGuid().ToString()))) ;

            // create the path and return it
            Directory.CreateDirectory(filePath);
            return filePath;
        }
    }
}
