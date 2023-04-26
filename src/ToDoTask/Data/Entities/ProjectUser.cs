using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ToDoTask.Data.Entities
{
    [Table("ProjectUsers")]
    public class ProjectUser
    {
       
        [Required]
        [DisplayName("Người Dùng")]
        public string UserId { get; set; }
        [Required]
        public string ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Job> Jobs { get; set; }
    }
}
