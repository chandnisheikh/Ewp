using Microsoft.AspNetCore.Mvc;

namespace Ewp.Models
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
