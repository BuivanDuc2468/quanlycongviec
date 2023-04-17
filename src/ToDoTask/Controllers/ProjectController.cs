using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoTask.Data;
using ToDoTask.Data.Entities;
using ToDoTask.Models.Contents;
using System.Security.Claims;
using ToDoTask.Models;

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
        public async Task<IActionResult> Index(string? searchString, int page=1)
        {

            var user = (from d in _context.Projects
                        join f in _context.Users on d.UserId equals f.Id
                        select new UserVm()
                        {
                            UserId = f.Id,
                            NameUser = f.Name
                        }).Distinct().ToList();
            ViewBag.User = user;
            const int pageSize = 10;
            if (page < 1)
                page = 1;
            List<Project> projects = await _context.Projects.ToListAsync();
            var recsCount = projects.Count();
            var pager = new Pager(recsCount, page, pageSize);
            int recSkip = (page - 1) * pageSize;
            if (!String.IsNullOrEmpty(searchString))
            {
                projects = projects.Where(n => n.Name.Contains(searchString) || n.Description.Contains(searchString)).ToList();
            }
            var data = projects.Skip(recSkip).Take(pager.PageSize).ToList();
            ViewBag.Pager = pager;
            ViewData["CurrentFilter"] = searchString;
            data = data.OrderByDescending(s => s.CreatedAt).ToList();
            return View(data);
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
                return NotFound("Project with id is not found");

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
                    return NotFound("Project is not found");

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
                    return BadRequest("Update project failed");
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
                return NotFound("Project with id: {id} is not found");
            _context.Projects.Remove(project);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Redirect("Index");
            }
            return View();
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
