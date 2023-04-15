using System.ComponentModel.DataAnnotations;

namespace ToDoTask.Models.Contents
{
    public class UserRoleView
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [Display(Name = "Role")]
        public string RoleName { get; set; }
        public string RoleId { get; set; }
    }
}
