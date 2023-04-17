using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoTask.Data;
using ToDoTask.Models.Contents;
using ToDoTask.Data.Entities;
using ToDoTask.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using ToDoTask.Models;

namespace ToDoTask.Controllers
{
    [Authorize]
    public class UserController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager,ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [Authorize]
        [Route("Nguoi-dung")]
        public async Task<IActionResult> Index(string? searchString, int page = 1)
        {
            var result = (from u in _context.Users
                         join ur in _context.UserRoles on u.Id equals ur.UserId
                         join r in _context.Roles on ur.RoleId equals r.Id
                         where r.Name != Roles.Admin.ToString()
                         select new UserRoleView {
                             UserId = u.Id,
                             Name = u.Name,
                             Email = u.Email, 
                             RoleName = r.Name,
                             RoleId = r.Id
                         }).ToList();
            const int pageSize = 10;
            if (page < 1)
                page = 1;
            var recsCount = result.Count();
            var pager = new Pager(recsCount, page, pageSize);
            int recSkip = (page - 1) * pageSize;
            if (!String.IsNullOrEmpty(searchString))
            {
                result = result.Where(n => n.Name.Contains(searchString) || n.Email.Contains(searchString)).ToList();
            }
            var data = result.Skip(recSkip).Take(pager.PageSize).ToList();
            ViewBag.Pager = pager;
            ViewData["CurrentFilter"] = searchString;
            return View(data);
        }
        [Authorize]
        
        public async Task<IActionResult> Edit(string id)
        {
            var result = from u in _context.Users
                         join ur in _context.UserRoles on u.Id equals ur.UserId
                         join r in _context.Roles on ur.RoleId equals r.Id
                         where u.Id == id
                         select new UserRoleView
                         {
                             UserId = u.Id,
                             Name = u.Name,
                             Email = u.Email,
                             RoleName = r.Name,
                             RoleId = r.Id
                         };
            var data = result.FirstOrDefault();
            var roles = await _context.Roles.ToListAsync();
            ViewBag.Roles = roles;
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(string id,string idr, UserRoleView userRole)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return RedirectToAction("Index");
            bool updateUser = false;
            if (userRole.Name != user.Name)
            {
                user.Name = userRole.Name;
                updateUser = true;
            }
            if (userRole.Email != user.Email)
            {
                user.Email = userRole.Email;
                updateUser = true;
            }
            if(idr != userRole.RoleId)
            {
                var role = await _roleManager.FindByIdAsync(userRole.RoleId);
                var users = await _userManager.FindByIdAsync(id);

                var userRoleTemp = await _context.UserRoles.FindAsync(id, idr);
                if (userRoleTemp != null)
                {
                    _context.UserRoles.Remove(userRoleTemp);
                }
                await _userManager.AddToRoleAsync(users, role.Name);
                
            }
            if(updateUser == true)
            {
                _context.Users.Update(user);
            }
            _context.SaveChanges();
            return RedirectToAction("Index","User");
        }
        public async Task<IActionResult> Delete(string Id)
        {
            var user = await _context.Users.FindAsync(Id);
            if (user == null)
                return RedirectToAction("Index");
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index","User");
        }
    }
}
