using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoTask.Data.Entities
{
    [Table("UserJobs")]
    public class UserJob
    {
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        [Required]
        public string UserId { get; set; }
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        [Required]
        public int JobId { get; set; }
        [Display(Name = "Ngày giao")]
        public DateTime DateAssign { get; set; }
        
    }
}
