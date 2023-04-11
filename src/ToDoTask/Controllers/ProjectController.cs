using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoTask.Data;
using ToDoTask.Data.Entities;
using ToDoTask.Models.Contents;
using System.Security.Claims;
namespace ToDoTask.Controllers
{
    public class ProjectController : Controller
    {

        private readonly ApplicationDbContext _context;
        
        public ProjectController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize]
        // GET: ProjectController
        public async Task<ActionResult> Index()
        {
            var project = await _context.Projects.ToListAsync();
            var projectvms = project.Select(c => ProjectVm(c)).ToArray();
            var user = await _context.Users.ToArrayAsync();
            ViewBag.User = user;
            return View(projectvms);
        }

        // GET: ProjectController/Create

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
                UserId= userId.Value
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
                 UserId = project.UserId
            };
        }
    }
}
