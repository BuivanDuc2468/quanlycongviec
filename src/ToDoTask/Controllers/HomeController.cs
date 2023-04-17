using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using ToDoTask.Constants;
using ToDoTask.Data;
using ToDoTask.Data.Entities;
using ToDoTask.Models;
using ToDoTask.Models.Contents;

namespace ToDoTask.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = _roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var records = await _dbContext.StatisticProcedure.FromSqlRaw("EXECUTE statisticalTaskOfUser").ToListAsync();
            var project = await _dbContext.Projects.ToListAsync();
            var Task = await _dbContext.Jobs.ToListAsync();
            var projectWaitting = project.Count(a=>a.Status == (int)Status.Waitting );
            var projectInprogress = project.Count(a=>a.Status == (int)Status.Processing);
            var projectDone = project.Count(a=>a.Status == (int)Status.Done);
            var jobWaitting = Task.Count(a => a.Status == (int)Status.Waitting);
            var jobInprogress = Task.Count(a => a.Status == (int)Status.Processing);
            var jobDone = Task.Count(a => a.Status == (int)Status.Done);
            var user = _userManager.GetUserAsync(User).Result;
            var totalProject = project.Count();
            var TotalTask = Task.Count();
            ViewBag.totalProject = totalProject;
            ViewBag.percentWaitting = Math.Floor((float)projectWaitting / totalProject * 100);
            ViewBag.percentInprogress = Math.Floor((float)projectInprogress / totalProject * 100);
            ViewBag.percentDone = Math.Floor((float)projectDone / totalProject * 100);
            ViewBag.TotalTask = TotalTask;
            ViewBag.percentWaittingTask = Math.Floor((float)jobWaitting / TotalTask * 100);
            ViewBag.percentInprogressTask = Math.Floor((float)jobInprogress / TotalTask * 100);
            ViewBag.percentDoneTask = Math.Floor((float)jobDone / TotalTask * 100);
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                var totalUser = await _dbContext.Users.CountAsync();
                ViewBag.TotalUser = totalUser;
            }
            return View(records);
            
        }
        [Authorize]
        public async Task<IActionResult> Statistical()
        {
            var records = await _dbContext.StatisticProcedure.FromSqlRaw("EXECUTE statisticalTaskOfUser").ToListAsync();
            return View(records);

        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}