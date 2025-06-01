using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<AuthorizationController> _logger;

        public HomeController(ILogger<AuthorizationController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
