using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using VisualRecognition.Mappers;
using VisualRecognition.Services;
using VisualRecognition.ViewModels;
using WatsonServices.Services;
using WatsonServices.Models.VisualRecognition;
using System.Linq;

namespace VisualRecognition.Controllers
{
    public class ApiController : Controller
    {
        private readonly IFileEncoderService _fileEncoderService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IVisualRecognitionService _visualRecognitionService;

        private const AcceptLanguage DefaultAcceptLanguage = AcceptLanguage.EN;
        private const double DefaultThreshold = 0.5f;

        public ApiController(IFileEncoderService fileEncoderService,
            IHostingEnvironment hostingEnvironment,
            IVisualRecognitionService visualRecognitionService)
        {
            _fileEncoderService = fileEncoderService;
            _hostingEnvironment = hostingEnvironment;
            _visualRecognitionService = visualRecognitionService;

            // set to false, or comment these lines to opt out of sharing data with Watson
            _visualRecognitionService.ShareData = true;
        }

        public async Task<IActionResult> Classifiers(string classifierId)
        {
            return new JsonResult(await _visualRecognitionService.GetClassifierAsync(classifierId));
        }

        // POST: api/classifiers
        [HttpPost]
        public async Task<IActionResult> Classifiers([FromBody]CreateClassifierViewModel viewModel)
        {
            if (viewModel == null)
            {
                return BadRequest("Missing view model");
            }

            // get zip file for each selected bundle
            ClassPositiveExamples[] positiveExamples = viewModel.Bundles
                .Where(m => !m.StartsWith("negative")).Select(m => new ClassPositiveExamples()
                {
                    ClassName = viewModel.Names[viewModel.Bundles.IndexOf(m)],
                    FileName = Path.Combine(_hostingEnvironment.WebRootPath, "images", "bundles", viewModel.Kind, m + ".zip")
                }).ToArray();
            var negativeBundle = viewModel.Bundles.FirstOrDefault(m => m.StartsWith("negative"));
            string negativeZipFile = null;
            if (negativeBundle != null)
            {
                negativeZipFile =
                    Path.Combine(_hostingEnvironment.WebRootPath, "images", "bundles", viewModel.Kind, negativeBundle + ".zip");
            }

            // call VisualRecognitionService and return the created classifier
            var result = new JsonResult(await _visualRecognitionService.CreateClassifierAsync(viewModel.Name, negativeZipFile, positiveExamples));

            // cleanup can happen after the result is returned to the front-end
            // disable the warning about this task continuing after the method ends
#pragma warning disable 4014
            Task.Run(() =>
            {
                // delete the classifier after 1 hour
                if (((Classifier)result?.Value)?.ClassifierId != null)
                {
                    var classifierId = ((Classifier)result.Value).ClassifierId;
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
            if (viewModel?.ImageByteContent?.Length > 0)
            {
                ClassifyResponse classifyResponse = null;
                ClassifyResponse facesResponse = null;
                ClassifyResponse wordsResponse = null;
                if (viewModel.IsUrl)
                {
                    classifyResponse = await _visualRecognitionService.ClassifyAsync(viewModel.Url, DefaultAcceptLanguage, DefaultThreshold,
                            ClassifierOwnerMapper.Map(viewModel.ClassifierOwners), viewModel.Classifier?.ClassifierId);
                    facesResponse = await _visualRecognitionService.RecognizeFacesAsync(viewModel.Url);
                    wordsResponse = await _visualRecognitionService.GetImageSceneTextAsync(viewModel.Url);
                }
                else
                {
                    classifyResponse = await _visualRecognitionService.ClassifyAsync(
                            viewModel.ImageFileName, viewModel.ImageByteContent, DefaultAcceptLanguage, DefaultThreshold,
                            ClassifierOwnerMapper.Map(viewModel.ClassifierOwners), viewModel.Classifier?.ClassifierId);
                    facesResponse = await _visualRecognitionService.RecognizeFacesAsync(viewModel.ImageFileName, viewModel.ImageByteContent);
                    wordsResponse = await _visualRecognitionService.GetImageSceneTextAsync(viewModel.ImageFileName, viewModel.ImageByteContent);
                }
                return new JsonResult(
                    ImagesMapper.Map(classifyResponse, facesResponse, wordsResponse)
                );
            }

            // byte content was null or 0 length
            return new BadRequestResult();
        }
    }
}
