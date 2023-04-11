using Microsoft.AspNetCore.Identity;
using ToDoTask.Constants;
using ToDoTask.Data.Entities;
using System;
namespace ToDoTask.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            var userManager = service.GetService<UserManager<User>>();
            var roleManager = service.GetService<RoleManager<IdentityRole>>();
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Manager.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));
            var user = new User
            {
                UserName = "Admin@gmail.com",
                Email = "Admin@gmail.com",
                Name = "Admin",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            var userInDb = await userManager.FindByEmailAsync(user.Email);
            if (userInDb == null)
            {
                await userManager.CreateAsync(user, "Admin@123");
                await userManager.AddToRoleAsync(user, Roles.Admin.ToString());
            }
            var user1 = new User
            {
                UserName = "User1@gmail.com",
                Email = "User1@gmail.com",
                Name = "User1",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            var userInDb1 = await userManager.FindByEmailAsync(user1.Email);
            if (userInDb1 == null)
            {
                await userManager.CreateAsync(user1, "Manager@123");
                await userManager.AddToRoleAsync(user1, Roles.Manager.ToString());
            }
            var user2 = new User
            {
                UserName = "User2@gmail.com",
                Email = "User2@gmail.com",
                Name = "User2",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            var userInDb2 = await userManager.FindByEmailAsync(user2.Email);
            if (userInDb2 == null)
            {
                await userManager.CreateAsync(user2, "User@123");
                await userManager.AddToRoleAsync(user2, Roles.User.ToString());
            }
            var user3 = new User
            {
                UserName = "User3@gmail.com",
                Email = "User3@gmail.com",
                Name = "User3",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            var userInDb3 = await userManager.FindByEmailAsync(user3.Email);
            if (userInDb3 == null)
            {
                await userManager.CreateAsync(user3, "User@123");
                await userManager.AddToRoleAsync(user3, Roles.User.ToString());
            }
        }
    }
}
