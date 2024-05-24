using AjaxDemo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AjaxDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyDbContext _context;

        public HomeController(ILogger<HomeController> logger, MyDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var category = _context.Categories/*.Where(p => p.CategoryId == 1)*/;
            return View(category);
        }

        public IActionResult Demo()
        {
            return View();
        }

        public IActionResult Demo2()
        {
            return View();
        }

        public IActionResult Address()
        {
            return View();
        }


        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Spots()
        {
            return View();
        }

        public IActionResult CallAPI()
        {
            return View();
        }

        public IActionResult History()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}