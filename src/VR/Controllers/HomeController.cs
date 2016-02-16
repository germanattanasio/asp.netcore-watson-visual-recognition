using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using VR.Models;
using VR.Services;
using Microsoft.AspNet.Mvc.Rendering;
using System.Collections.Generic;

namespace VR.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWatsonVRService _vrService;
        private readonly IApplicationEnvironment _env;
        private const int DefaultMaxScores = 5;

        public HomeController(IApplicationEnvironment env, IWatsonVRService vrService)
        {
            _env = env;
            _vrService = vrService;
            _vrService.ShareData = true; // set to false, or comment this line to opt out of sharing data with Watson
        }

        public async Task<IActionResult> Index(string classifierId = null, int? maxScores = null)
        {
            var viewModel = new WatsonVRViewModel();
            viewModel.MaxScores = maxScores.HasValue ? maxScores.Value : DefaultMaxScores;
            viewModel.ClassifierIds = await GetClassifierSelectList(classifierId);
            if (!viewModel.ClassifierIds.Any())
            {
                ModelState.AddModelError("ClassifierIds", "Failed to get classifier list from Watson service");
            }
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(WatsonVRViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new WatsonVRViewModel();
                ModelState.AddModelError("viewModel", "Form must be filled out completely before submitting");
            }

            string fileName = "";
            string fileExt = "";

            if (viewModel.ImageUpload != null)
            {
                fileName = ContentDispositionHeaderValue.Parse(viewModel.ImageUpload.ContentDisposition).FileName.Trim('"');
                fileExt = Path.GetExtension(fileName).ToLowerInvariant();
            }

            if (fileExt != ".jpg" && fileExt != ".jpeg" && fileExt != ".zip" && fileExt != ".png")
            {
                ModelState.AddModelError("ImageUpload", "Must specify a jpeg, png, or zip file");
            }

            if (ModelState.IsValid)
            {
                var filePath = GetTempFilename(fileExt);
                await viewModel.ImageUpload.SaveAsAsync(filePath);
                var results = (await _vrService.ClassifyAsync(filePath, viewModel.MaxScores,
                    viewModel.ClassifierId != null ? new string[] { viewModel.ClassifierId } : null));
                if (results != null)
                {
                    viewModel.ImageResults = results.ImageResults;
                }
                else
                {
                    ModelState.AddModelError("Imageresults", "Failed to retrieve results from Watson service");
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
            var viewModel = new WatsonVRManageClassifierViewModel();
            viewModel.ClassifierIds = await GetClassifierSelectList(null);
            if (!viewModel.ClassifierIds.Any())
            {
                ModelState.AddModelError("ClassifierIds", "Failed to get classifier list from Watson service");
            }
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(WatsonVRManageClassifierViewModel viewModel)
        {
            if (viewModel == null || viewModel.ActionType == WatsonVRClassifierAction.None)
            {
                ModelState.AddModelError("viewModel", "Form must be filled out completely before submitting");
                var returnModel = new WatsonVRManageClassifierViewModel();
                returnModel.ClassifierIds = await GetClassifierSelectList(null);
                if (!returnModel.ClassifierIds.Any())
                {
                    ModelState.AddModelError("ClassifierIds", "Failed to get classifier list from Watson service");
                }
                return View(returnModel);
            }

            if (viewModel.ActionType == WatsonVRClassifierAction.Create)
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
            else if (viewModel.ActionType == WatsonVRClassifierAction.Delete)
            {
                if (string.IsNullOrEmpty(viewModel.ClassifierId))
                {
                    ModelState.AddModelError("ClassifierId", "Must specify a Classifier Id to delete");
                }
            }

            if (ModelState.IsValid && viewModel.ActionType == WatsonVRClassifierAction.Create)
            {
                var tempPositiveName = GetTempFilename(".zip");
                viewModel.PositiveExamples.SaveAs(tempPositiveName);
                var tempNegativeName = GetTempFilename(".zip");
                viewModel.NegativeExamples.SaveAs(tempNegativeName);

                var classifierVm = await _vrService.CreateClassifierAsync(tempPositiveName, tempNegativeName, viewModel.ClassifierName);
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
            else if (ModelState.IsValid && viewModel.ActionType == WatsonVRClassifierAction.Delete)
            {
                viewModel.Success = await _vrService.DeleteClassifierAsync(viewModel.ClassifierId);
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
            var classifiers = await _vrService.GetClassifiersAsync();
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