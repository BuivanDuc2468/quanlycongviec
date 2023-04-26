using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDoTask.Data.Entities;
using ToDoTask.Models.Contents;

namespace ToDoTask.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> option)
            : base(option)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            foreach(var entityType in modelBuilder.Model.GetEntityTypes()) {

                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }
            modelBuilder.Entity<IdentityRole>().Property(x => x.Id).HasMaxLength(50).IsUnicode(false);
            modelBuilder.Entity<ProjectUser>()
                       .HasKey(c => new {c.UserId, c.ProjectId });

            modelBuilder.Entity<ProjectUser>()
                .HasOne(pc => pc.Project)
                .WithMany(p => p.ProjectUsers)
                .HasForeignKey(pc => pc.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            modelBuilder.Entity<ProjectUser>()
                .HasOne(pc => pc.User)
                .WithMany(c => c.ProjectUsers)
                .HasForeignKey(pc => pc.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            modelBuilder.HasSequence("TodoTaskSequence");
            modelBuilder.Entity<Statistics>().HasNoKey();
            modelBuilder.Entity<StatisticPercent>().HasNoKey();
            modelBuilder.Entity<Job>()
            .HasOne(j => j.ProjectUser)
            .WithMany(p => p.Jobs)
            .HasForeignKey(p => new { p.UserId, p.ProjectId})
            .OnDelete(DeleteBehavior.ClientSetNull);


        }
        public DbSet<Project> Projects { set; get; }
        public DbSet<Job> Jobs { set; get; }
        public DbSet<ProjectUser> ProjectUsers { set; get; }
        public DbSet<Statistics> StatisticProcedures { set; get; }
        public DbSet<StatisticPercent> StatisticPercents { set; get; }
        public DbSet<ProjectVm> ProjectVm { get; set; } = default!;
    }
}