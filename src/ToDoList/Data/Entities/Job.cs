using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Data.Entities
{
    [Table("Jobs")]
    public class Job
    {
        [Key]
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string Id { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        [Required]
        public string ProjectId { get; set; }

        [MaxLength(200)]
        [Required]
        public string Name { get; set; }
        
        [Required]
        public int Status { get; set; }

        public DateTime DateToDo { get; set; }
        public DateTime DateComplete { get; set; }

    }
}
