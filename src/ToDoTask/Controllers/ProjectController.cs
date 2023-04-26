using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoTask.Data;
using ToDoTask.Data.Entities;
using ToDoTask.Models.Contents;
using System.Security.Claims;
using System.Drawing.Printing;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ToDoTask.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ProjectController : Controller
    {

        private readonly ApplicationDbContext _context;

        public ProjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page)
        {
            List<Project> projects = null;
            var user = (from d in _context.Projects join f in _context.Users on d.UserId equals f.Id select new UserVm()
            {
                UserId = f.Id,
                NameUser = f.Name
            }).Distinct().ToList();
            ViewBag.User = user;
            const int pageSize = 10;
            int totalProject = await _context.Projects.CountAsync();
            if (totalProject > 0)
            {
                int countPage = (int)Math.Ceiling((double)totalProject / pageSize);
                if (page < 1)
                    page = 1;
                if (page > countPage)
                    page = countPage;

                var query = (from project in _context.Projects
                             orderby project.DateLine descending
                             select project).Skip((page - 1) * pageSize).Take(pageSize);
                projects = await query.ToListAsync();

                ViewBag.CountPage = countPage;
                ViewBag.CurrentPage = page;
                return View(projects);
            }
            else
            {
                return View(projects);
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(ProjectCreateVm request)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier);
            var project = new Project()
            {
                Name = request.Name,
                Description = request.Description,
                Status = 0,
                CreatedAt = DateTime.Now,
                UserId = userId.Value,
                DateLine = request.DateLine
            };
            _context.Projects.Add(project);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return RedirectToAction("Index", "Project");
            }
            else
            {
                return BadRequest("Create project failed");
            }
        }

        public async Task<ActionResult> Edit(string id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return Content("Project with id is not found");

            ProjectVm projectVm = ProjectVm(project);
            return View(projectVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, ProjectVm request)
        {

            try
            {
                var project = await _context.Projects.FindAsync(id);
                if (project == null)
                    return Content("Project is not found");

                project.Name = request.Name;
                project.Description = request.Description;
                project.DateLine = request.DateLine;
                project.Status = request.Status;
                _context.Projects.Update(project);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    return RedirectToAction("Index", "Project");
                }
                else
                {
                    return Content("Update project failed");
                }
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var project = await _context.Projects.FindAsync(id);
                if (project == null)
                    return BadRequest("Project is not found.");
                _context.Projects.Remove(project);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    return Content("Success.");
                }
                return BadRequest("Delete Error.");
            }
            catch
            {
                return View();
            }
        }
        private static ProjectVm ProjectVm(Project project)
        {
            return new ProjectVm()
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Status = project.Status,
                CreatedAt = project.CreatedAt,
                UserId = project.UserId,
                DateLine = project.DateLine
            };
        }
        [HttpGet]
        public async Task<IActionResult> Search(string? keySearch)
        {
            if (!string.IsNullOrEmpty(keySearch)){
                var data = (from p in _context.Projects
                            join user in _context.Users on p.UserId equals user.Id
                            where ((p.Name.Contains(keySearch)) || (p.Description.Contains(keySearch)))
                            select new
                            {
                                id = p.Id,
                                name = p.Name,
                                createdAt = p.CreatedAt.ToString().Substring(0, p.CreatedAt.ToString().Length - 8),
                                dateLine = p.DateLine.ToString().Substring(0, p.CreatedAt.ToString().Length - 8),
                                userName = user.Name,
                                status = p.Status == 0 ? "Waitting" : p.Status == 1 ? "Processing" : "Done"
                            }).Take(10);
                var data2 = await data.ToListAsync();
                return Json(data2);
            }
            else
            {
                return null;
            }
        }
    }
}
