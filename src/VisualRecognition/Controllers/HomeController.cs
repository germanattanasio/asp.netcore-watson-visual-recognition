using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using VisualRecognition.ViewModels;

namespace VisualRecognition.Controllers
{
    public class HomeController : Controller
    {
        private const string CookiesBundleKey = "bundle";
        private const string CookiesClassifierKey = "classifier";

        public IActionResult Index()
        {
            SampleImagesViewModel viewModel = new SampleImagesViewModel()
            {
                Class = SampleImagesClassEnum.Use,
                BundleId = SampleImageBundles.DefaultBundleId,
                BundleImages = SampleImageBundles.ImageBundles[SampleImageBundles.DefaultBundleId]
            };
            return View(viewModel);
        }

        public IActionResult Test()
        {
            // if bundle id wasn't specified, redirect to the index page
            if (!HttpContext.Request.Cookies.ContainsKey(CookiesBundleKey))
            {
                return RedirectToAction("Index");
            }
            string bundleId = "";
            try
            {
                bundleId = JsonConvert.DeserializeObject<CreateClassifierViewModel>(
                        HttpContext.Request.Cookies[CookiesBundleKey]).Kind;
                if (!SampleImageBundles.ImageBundles.ContainsKey(bundleId))
                {
                    throw new ArgumentNullException("bundle");
                }
            }
            catch (Exception)
            {
                // if the bundle id was invalid, redirect to index
                return RedirectToAction("Index");
            }

            var classifierJson = HttpContext.Request.Cookies[CookiesClassifierKey];
            ClassifierViewModel classifier = null;
            try
            {
                classifier = JsonConvert.DeserializeObject<ClassifierViewModel>(classifierJson);
            }
            catch (Exception) { /* ignore the exception, leaving classifier as null */ }

            TestClassifierViewModel viewModel = new TestClassifierViewModel()
            {
                ClassifierName = string.IsNullOrEmpty(classifier?.Name) ? "" : classifier.Name,
                Samples = new SampleImagesViewModel()
                {
                    Class = SampleImagesClassEnum.Test,
                    BundleId = bundleId,
                    BundleImages = SampleImageBundles.ImageBundles[bundleId]
                }
            };
            return View(viewModel);
        }

        public IActionResult Train()
        {
            return View();
        }
    }
}