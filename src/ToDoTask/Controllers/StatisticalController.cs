using Microsoft.AspNetCore.Mvc;

namespace ToDoTask.Controllers
{
    public class StatisticalController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
