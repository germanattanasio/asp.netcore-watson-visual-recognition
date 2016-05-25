using Microsoft.AspNetCore.Mvc;

namespace VisualRecognition.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Test()
        {
            return View();
        }

        public IActionResult Train()
        {
            return View();
        }
    }
}