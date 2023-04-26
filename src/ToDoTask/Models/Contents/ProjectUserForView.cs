using System.ComponentModel.DataAnnotations;

namespace ToDoTask.Models.Contents
{
    public class ProjectUserForView
    {
        [Display(Name = "Tên Dự Án")]
        public string ProjectId { get; set; }
        [Display(Name = "Người dùng")]
        public List<string> Users { get; set; }
    }
}
