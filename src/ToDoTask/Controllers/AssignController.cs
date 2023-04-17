using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using ToDoTask.Data;
using ToDoTask.Data.Entities;
using ToDoTask.Models.Contents;

namespace ToDoTask.Controllers
{
    [Authorize]
    [Authorize(Roles = "Admin,Manager")]
    public class AssignController : Controller
    {     
        private readonly ApplicationDbContext _context;
        public AssignController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize]
        public async Task<ActionResult> Index()
        {
            var projectUser = await _context.ProjectUsers.ToArrayAsync();
            var project = await _context.Projects.ToArrayAsync();
            var projectvms = project.Select(c => ProjectVm(c)).ToArray();
            var user = await _context.Users.ToArrayAsync();
            ViewBag.Project = projectvms;
            ViewBag.User = user;
            return View(projectUser);
        }
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier);
            var project = await _context.Projects.ToArrayAsync();
            var user = await _context.Users.Where(s => !s.Name.Contains("Admin") && s.Id != userId.Value).ToArrayAsync();
            ViewBag.Project = project;
            ViewBag.User = user;
            return View();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(ProjectUserCreateVm request)
        {
            try
            {
                var a = request.UserId;
                var b = request.ProjectId;
                if (a.Count > 0)
                {
                    foreach(var i in a)
                    {
                        var existProjectUser = await _context.ProjectUsers.FindAsync(i, b);
                        if(existProjectUser == null)
                        {
                            var projectUser = new ProjectUser()
                            {
                                ProjectId = b,
                                UserId = i,
                            };
                            _context.ProjectUsers.Add(projectUser);
                            var result = await _context.SaveChangesAsync();
                        }
                    };
                }
                return RedirectToAction("Index", "Assign");
            }
            catch
            {
                return RedirectToAction("Index", "Assign");
            }
        }
        [Authorize]
        public async Task<IActionResult> Delete(string id,string id2)
        {

            var projectUser = await _context.ProjectUsers.FindAsync(id,id2);
            if (projectUser == null)
                return NotFound("ProjectUser is not found");
            _context.ProjectUsers.Remove(projectUser);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return RedirectToAction("Index","Assign");
            }
            return View();
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
                UserId = project.UserId
            };
        }
    }
}
