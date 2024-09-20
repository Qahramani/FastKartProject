using Microsoft.AspNetCore.Mvc;

namespace FastKartProject.Controllers
{
    public class WishlistController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
