using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Rendering;
using System.Collections.Generic;
using VisualRecognition.Mappers;
using VisualRecognition.Services;
using VisualRecognition.ViewModels;
using WatsonServices.Services;

namespace VisualRecognition.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAlchemyVisionService _alchemyVisionService;
        private readonly IVisualRecognitionService _visualRecognitionService;
        private readonly IFileEncoderService _encService;
        private readonly IApplicationEnvironment _env;
        private const int DefaultMaxScores = 5;

        public HomeController(IApplicationEnvironment env, IVisualRecognitionService visualRecognitionService,
            IAlchemyVisionService alchemyVisionService, IFileEncoderService encService)
        {
            _alchemyVisionService = alchemyVisionService;
            _env = env;
            _encService = encService;
            _visualRecognitionService = visualRecognitionService;

            // set to false, or comment these lines to opt out of sharing data with Watson
            _alchemyVisionService.ShareData = true;
            _visualRecognitionService.ShareData = true;
        }

        public async Task<IActionResult> Index(string classifierId = null, int? maxScores = null)
        {
            var viewModel = new VisualRecognitionViewModel();
            viewModel.MaxScores = maxScores.HasValue ? maxScores.Value : DefaultMaxScores;
            viewModel.ClassifierIds = await GetClassifierSelectList(classifierId);
            if (!viewModel.ClassifierIds.Any())
            {
                ModelState.AddModelError("ClassifierIds", "Failed to get classifier list from Watson service");
            }
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(VisualRecognitionViewModel viewModel)
        {
            // select which service will be used to handle this request
            // if ClassifierId is null, use AlchemyVision, otherwise use Visual Recognition
            var service = (viewModel.ClassifierId == null) ? (IWatsonFileService)_alchemyVisionService : (IWatsonFileService)_visualRecognitionService;

            if (viewModel == null)
            {
                viewModel = new VisualRecognitionViewModel();
                ModelState.AddModelError("viewModel", "Form must be filled out completely before submitting");
            }

            string fileName = "";
            string fileExt = "";

            if (viewModel.ImageUpload != null)
            {
                fileName = ContentDispositionHeaderValue.Parse(viewModel.ImageUpload.ContentDisposition).FileName.Trim('"');
                fileExt = Path.GetExtension(fileName).ToLowerInvariant();
            }

            if (!service.SupportsFileExtension(fileExt))
            {
                ModelState.AddModelError("ImageUpload", "Must specify a gif, jpeg, png, or zip file");
            }

            if (ModelState.IsValid)
            {
                var filePath = GetTempFilename(fileExt);
                await viewModel.ImageUpload.SaveAsAsync(filePath);

                Dictionary<string, string> base64Images = null;

                // create and run a new task to encode the image file(s) in the background
                Task encodeImages = new TaskFactory().StartNew(async () =>
                {
                    base64Images = await _encService.EncodeZipFileAsync(filePath);
                });

                if (service.GetType() == _visualRecognitionService.GetType())
                {
                    // use VisualRecognition service
                    var results = (await _visualRecognitionService.ClassifyAsync(filePath, viewModel.ClassifierId));
                    // wait for the image encoding to finish if it is not done yet
                    Task.WaitAll(encodeImages);

                    // use the ImagesMapper to map the response from Watson to the base64 images and original filenames
                    ImagesMapper.Map(results, viewModel, base64Images, viewModel.MaxScores);
                }
                else
                {
                    // use AlchemyVision service
                    var results = (await _alchemyVisionService.GetImageKeywordsAsync(filePath, false));
                    // wait for the image encoding to finish if it is not done yet
                    Task.WaitAll(encodeImages);

                    // use the ImagesMapper to map the response from Watson to the base64 images and original filenames
                    ImagesMapper.Map(results, viewModel, fileName, Path.GetFileName(filePath), base64Images, viewModel.MaxScores);
                }

                if (viewModel.ImageResults == null || !viewModel.ImageResults.Any())
                {
                    ModelState.AddModelError("ImageResults", "Failed to retrieve results from Watson service");
                }
                System.IO.File.Delete(filePath);
            }

            viewModel.ClassifierIds = await GetClassifierSelectList(null);
            if (!viewModel.ClassifierIds.Any())
            {
                ModelState.AddModelError("ClassifierIds", "Failed to get classifier list from Watson service");
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Manage()
        {
            var viewModel = new ManageClassifierViewModel();
            viewModel.ClassifierIds = await GetClassifierSelectList(null);
            if (!viewModel.ClassifierIds.Any())
            {
                ModelState.AddModelError("ClassifierIds", "Failed to get classifier list from Watson service");
            }
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(ManageClassifierViewModel viewModel)
        {
            if (viewModel == null || viewModel.ActionType == ClassifierActionType.None)
            {
                ModelState.AddModelError("viewModel", "Form must be filled out completely before submitting");
                var returnModel = new ManageClassifierViewModel();
                returnModel.ClassifierIds = await GetClassifierSelectList(null);
                if (!returnModel.ClassifierIds.Any())
                {
                    ModelState.AddModelError("ClassifierIds", "Failed to get classifier list from Watson service");
                }
                return View(returnModel);
            }

            if (viewModel.ActionType == ClassifierActionType.Create)
            {
                if (viewModel.PositiveExamples == null)
                {
                    ModelState.AddModelError("PositiveExamples", "Must upload a .zip file of positive examples");
                }
                if (viewModel.NegativeExamples == null)
                {
                    ModelState.AddModelError("NegativeExamples", "Must upload a .zip file of negative examples");
                }
                if (string.IsNullOrEmpty(viewModel.ClassifierName))
                {
                    ModelState.AddModelError("ClassifierName", "Must specify a name for this classifier");
                }

                if (viewModel.PositiveExamples != null)
                {
                    var positiveName = ContentDispositionHeaderValue.Parse(viewModel.PositiveExamples.ContentDisposition)
                        .FileName.Trim('"');
                    if (Path.GetExtension(positiveName).ToLowerInvariant() != ".zip")
                    {
                        ModelState.AddModelError("PositiveExamples", "Positive examples must be in a .zip file");
                    }
                }
                if (viewModel.NegativeExamples != null)
                {
                    var negativeName = ContentDispositionHeaderValue.Parse(viewModel.NegativeExamples.ContentDisposition)
                        .FileName.Trim('"');
                    if (Path.GetExtension(negativeName).ToLowerInvariant() != ".zip")
                    {
                        ModelState.AddModelError("NegativeExamples", "Negative examples must be in a .zip file");
                    }
                }
            }
            else if (viewModel.ActionType == ClassifierActionType.Delete)
            {
                if (string.IsNullOrEmpty(viewModel.ClassifierId))
                {
                    ModelState.AddModelError("ClassifierId", "Must specify a Classifier Id to delete");
                }
            }

            if (ModelState.IsValid && viewModel.ActionType == ClassifierActionType.Create)
            {
                var tempPositiveName = GetTempFilename(".zip");
                viewModel.PositiveExamples.SaveAs(tempPositiveName);
                var tempNegativeName = GetTempFilename(".zip");
                viewModel.NegativeExamples.SaveAs(tempNegativeName);

                // call the VisualRecognition service to create the classifier and then map that to a view model
                var classifierVm = ClassifierMapper.Map(
                    await _visualRecognitionService.CreateClassifierAsync(tempPositiveName, tempNegativeName, viewModel.ClassifierName));

                viewModel.ClassifierId = classifierVm != null ? classifierVm.ClassifierId : "";
                viewModel.Success = !string.IsNullOrEmpty(viewModel.ClassifierId);

                if (!viewModel.Success)
                {
                    ModelState.AddModelError("IWatsonVRService", "Unable to communicate with Watson service");
                }

                // cleanup temporary files
                System.IO.File.Delete(tempPositiveName);
                System.IO.File.Delete(tempNegativeName);
            }
            else if (ModelState.IsValid && viewModel.ActionType == ClassifierActionType.Delete)
            {
                viewModel.Success = await _visualRecognitionService.DeleteClassifierAsync(viewModel.ClassifierId);
            }

            viewModel.ClassifierIds = await GetClassifierSelectList(null);
            if (!viewModel.ClassifierIds.Any())
            {
                ModelState.AddModelError("ClassifierIds", "Failed to get classifier list from Watson service");
            }

            return View(viewModel);
        }

        private async Task<IEnumerable<SelectListItem>> GetClassifierSelectList(string selectedClassifierId)
        {
            var classifiers = await _visualRecognitionService.GetClassifiersAsync();
            if (classifiers != null) {
                return classifiers.Classifiers
                    .Select(m =>
                    new SelectListItem
                    {
                        Selected = m.ClassifierId == selectedClassifierId,
                        Value = m.ClassifierId,
                        Text = m.Name
                    }).ToList();
            }

            return new List<SelectListItem>();
        }

        private string GetTempFilename(string ext)
        {
            string filePath = Path.Combine(_env.ApplicationBasePath, "temp_files");
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string fileName;
            while (System.IO.File.Exists((fileName = Path.Combine(filePath, Guid.NewGuid() + "-" + DateTime.UtcNow.Ticks + ext)))) {}
            return fileName;
        }
    }
}