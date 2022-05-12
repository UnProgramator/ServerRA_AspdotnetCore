using Microsoft.AspNetCore.Mvc;

namespace ServerRA_AspnetCore.Controllers
{
    public class RepairsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
