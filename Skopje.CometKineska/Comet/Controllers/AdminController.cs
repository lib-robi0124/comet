using Microsoft.AspNetCore.Mvc;

namespace Comet.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
