// Controllers/ExampleController.cs
using Microsoft.AspNetCore.Mvc;

namespace Matcha_project.Controllers
{
    public class ExampleController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Message = "This is an example message.";
            return View();
        }
        
        public void ExampleMethod()
        {
            Console.WriteLine("This is an example method.");
        }
    }
}