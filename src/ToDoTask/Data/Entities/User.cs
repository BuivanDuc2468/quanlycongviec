using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoTask.Data.Entities
{
    public class User: IdentityUser
    {
        public string? Name { get; set; }
        public string? ProfilePicture { get; set; }
        public virtual ICollection<Job> Jobs { get; set; }
        public virtual ICollection<ProjectUser> ProjectUsers { get; set; }
    }
}
