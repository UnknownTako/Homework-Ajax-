using Microsoft.AspNetCore.Mvc;

namespace AjaxDemo.Controllers
{
    public class HomeWorkController : Controller
    {
        public IActionResult homework1()
        {
            return View();
        }

        public IActionResult homework2()
        {
            return View();
        }

        public IActionResult homework3()
        {
            return View();
        }

        public IActionResult homework4()
        {
            return RedirectToAction("spots","Home");
        }
    }
}
