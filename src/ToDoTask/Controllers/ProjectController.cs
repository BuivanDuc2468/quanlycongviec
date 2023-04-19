using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoTask.Data;
using ToDoTask.Data.Entities;
using ToDoTask.Models.Contents;
using System.Security.Claims;
using ToDoTask.Models;
using ToDoTask.Helpers;

namespace ToDoTask.Controllers
{
    [Authorize]
    [Authorize(Roles = "Admin,Manager") ]
    public class ProjectController : Controller
    {

        private readonly ApplicationDbContext _context;
        
        public ProjectController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // GET: ProjectController
        [Authorize]
        public async Task<IActionResult> Index(string? searchString, int page)
        {
            List<Project> projects;
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
                if (!String.IsNullOrEmpty(searchString))
                {
                    var query = (from project in _context.Projects
                             .Where(n => n.Name.Contains(searchString) || n.Description.Contains(searchString))
                                 orderby project.DateLine descending
                                 select project).Skip((page - 1) * pageSize).Take(pageSize);
                    projects = await query.ToListAsync();
                }
                else
                {
                    var query = (from project in _context.Projects
                                 orderby project.DateLine descending
                                 select project).Skip((page - 1) * pageSize).Take(pageSize);
                    projects = await query.ToListAsync();
                }
            
                ViewData["CurrentFilter"] = searchString;
                ViewBag.CountPage = countPage;
                ViewBag.CurrentPage = page;
                return View(projects);
            }
            else
            {
                return RedirectToAction("Create", "Project");
            }
        }

        public ActionResult Create()
        {
          return View();
        }

        // POST: ProjectController/Create
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
                UserId= userId.Value,
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

        // GET: ProjectController/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return Content("Project with id is not found");

            ProjectVm projectVm = ProjectVm(project);
            return View(projectVm);
        }

        // POST: ProjectController/Edit/5
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
                    return RedirectToAction("Index","Project");
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

        // GET: ProjectController/Delete/5
        public async Task<ActionResult> Delete(string id)

        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return Content("Project with id: {id} is not found");
            _context.Projects.Remove(project);
            var result = await _context.SaveChangesAsync();
            
            return RedirectToAction("Index","Project");
        }

        // POST: ProjectController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
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
                 Id= project.Id,
                 Name = project.Name ,   
                 Description = project.Description,
                 Status =project.Status,
                 CreatedAt= project.CreatedAt,
                 UserId = project.UserId,
                 DateLine=project.DateLine
            };
        }
        
    }
}
