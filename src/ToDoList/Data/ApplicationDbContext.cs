using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDoList.Data.Entities;

namespace ToDoList.Data
{
    //
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().Property(x => x.Id).HasMaxLength(50).IsUnicode(false);
            builder.Entity<User>().Property(x => x.Id).HasMaxLength(50).IsUnicode(false);

            builder.Entity<UserJob>()
                       .HasKey(c => new { c.JobId, c.UserId });
            builder.Entity<ProjectUser>()
                       .HasKey(c => new { c.UserId, c.ProjectId });
        }

        public DbSet<Project> Projects { set; get; }
        public DbSet<Job> Jobs { set; get; }
        public DbSet<Permission> Permissions { set; get; }
        public DbSet<UserJob> UserJobs { set; get; }
        public DbSet<ProjectUser> ProjectUsers { set; get; }

    }
}