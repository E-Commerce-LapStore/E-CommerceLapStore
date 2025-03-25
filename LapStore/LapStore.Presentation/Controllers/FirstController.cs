using Microsoft.AspNetCore.Mvc;

namespace LapStore.Presentation.Controllers
{
    public class FirstController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
