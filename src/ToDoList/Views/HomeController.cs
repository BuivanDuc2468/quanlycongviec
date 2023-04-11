using Microsoft.AspNetCore.Mvc;

namespace ToDoList.Views
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
