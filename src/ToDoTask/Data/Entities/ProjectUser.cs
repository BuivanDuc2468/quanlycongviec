using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ToDoTask.Data.Entities
{
    [Table("ProjectUsers")]
    public class ProjectUser
    {
       
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        [Required]
        [DisplayName("User")]
        public string UserId { get; set; }
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        [Required]
        [DisplayName("Project")]
        public string ProjectId { get; set; }
    }
}
