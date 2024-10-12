using Microsoft.AspNetCore.Mvc;

namespace CoreMasterDetails.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
