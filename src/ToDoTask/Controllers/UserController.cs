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
    [Authorize(Roles = "Admin")]
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
        public async Task<IActionResult> Index(string? searchString, int page)
        {
            List<UserRoleView> userRoleList;
            
            const int pageSize = 10;
            int totalUser = await _context.Users.CountAsync();
            int countPage = (int)Math.Ceiling((double)totalUser / pageSize);
            if (page < 1)
                page = 1;
            if (page > countPage)
                page = countPage;
            if (!String.IsNullOrEmpty(searchString))
            {
                var result = (from u in _context.Users
                              join ur in _context.UserRoles on u.Id equals ur.UserId
                              join r in _context.Roles on ur.RoleId equals r.Id
                              where( r.Name != Roles.Admin.ToString() &&( u.Name.Contains(searchString) || u.Email.Contains(searchString)))
                              select new UserRoleView
                              {
                                  UserId = u.Id,
                                  Name = u.Name,
                                  Email = u.Email,
                                  RoleName = r.Name,
                                  RoleId = r.Id
                              }).Skip((page - 1) * pageSize).Take(pageSize);
                userRoleList = await result.ToListAsync();
            }
            else
            {
                var result = (from u in _context.Users
                              join ur in _context.UserRoles on u.Id equals ur.UserId
                              join r in _context.Roles on ur.RoleId equals r.Id
                              where (r.Name != Roles.Admin.ToString())
                              select new UserRoleView
                              {
                                  UserId = u.Id,
                                  Name = u.Name,
                                  Email = u.Email,
                                  RoleName = r.Name,
                                  RoleId = r.Id
                              }).Skip((page - 1) * pageSize).Take(pageSize);
                userRoleList = await result.ToListAsync();
            }
            ViewData["CurrentFilter"] = searchString;
            ViewBag.CountPage = countPage;
            ViewBag.CurrentPage = page;
            return View(userRoleList);
        }
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var roles = await _context.Roles.ToListAsync();
            if(roles != null)
            ViewBag.Roles = roles;
            return View();
        } 
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(UserCreate userCreate)
        {
            if(ModelState != null)
            {
                var userC = new User
                {
                    UserName = userCreate.UserName,
                    Email = userCreate.Email,
                    Name = userCreate.Name,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };
                var userInDb = await _userManager.FindByEmailAsync(userC.Email);
                if (userInDb == null)
                {
                    await _userManager.CreateAsync(userC, userCreate.Password);
                    await _userManager.AddToRoleAsync(userC, userCreate.Role);
                }
            }
            return RedirectToAction("Index","User");
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
