using System.ComponentModel.DataAnnotations;

namespace ToDoTask.Models.Contents
{
    public class UserRoleView
    {
        public string UserId { get; set; }
        [Display(Name= "Tên")]
        public string Name { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Quyền")]
        public string RoleName { get; set; }
        public string RoleId { get; set; }
    }
}
