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
            var user = _userManager.GetUserAsync(User).Result;
            var totalTask = await _dbContext.Jobs.CountAsync();
            var totalProject = await _dbContext.Projects.CountAsync();
            ViewBag.TotalProject = totalProject;
            ViewBag.TotalTask = totalTask;
            var percentJob = await _dbContext.StatisticPercents.FromSqlRaw("EXECUTE PercentOfJob").ToListAsync();
            var percentProject = await _dbContext.StatisticPercents.FromSqlRaw("EXECUTE PercentOfProject").ToListAsync();
            StatisticPercent prJob = percentJob[0];
            StatisticPercent prProject = percentProject[0];
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                var totalUser = await _dbContext.Users.CountAsync();
                ViewBag.TotalUser = totalUser;
            }
            StatisticPercentForView model = new StatisticPercentForView
            {
                PercentJob = prJob,
                PercentProject = prProject
            };
            return View(model);
            
        }
        [Authorize]
        public async Task<IActionResult> Statistical()
        {
            var records = await _dbContext.StatisticProcedures.FromSqlRaw("EXECUTE statisticalTaskOfUser").ToListAsync();
            return View(records);

        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}