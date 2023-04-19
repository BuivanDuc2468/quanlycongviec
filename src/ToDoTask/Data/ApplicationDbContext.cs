using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDoTask.Data.Entities;
using ToDoTask.Models.Contents;

namespace ToDoTask.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().Property(x => x.Id).HasMaxLength(50).IsUnicode(false);
            builder.Entity<ProjectUser>()
                       .HasKey(c => new {c.UserId, c.ProjectId });
            builder.HasSequence("TodoTaskSequence");
            builder.Entity<Statistics>().HasNoKey();
            builder.Entity<StatisticPercent>().HasNoKey();

        }

        public DbSet<Project> Projects { set; get; }
       
        public DbSet<Job> Jobs { set; get; }
        public DbSet<Permission> Permissions { set; get; }
        public DbSet<ProjectUser> ProjectUsers { set; get; }
        public DbSet<Statistics> StatisticProcedures { set; get; }
        public DbSet<StatisticPercent> StatisticPercents { set; get; }
        public DbSet<ProjectVm> ProjectVm { get; set; } = default!;
    }
}