using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AcrylicGame.Models;

namespace AcrylicGame.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Home landing page
        public IActionResult Index()
        {
            return View();
        }

        // Privacy policy page
        public IActionResult Privacy()
        {
            return View();
        }

        // Optional About page
        public IActionResult About()
        {
            return View();
        }

        // Error handler
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}

