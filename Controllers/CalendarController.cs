using Microsoft.AspNetCore.Mvc;

namespace TeamProject.Controllers
{
    public class CalendarController : Controller
    {
        public IActionResult Index()
        {
            ViewData["CurrentTab"] = "Calendar";
            return View();
        }
    }
}
