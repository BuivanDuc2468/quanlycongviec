using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ToDoTask.Data.Entities
{
    [Table("Jobs")]
    public class Job
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        [Required]
        [Display(Name = "Project")]
        public string ProjectId { get; set; }
        [MaxLength(200)]
        [Required]
        public string Name { get; set; }
        public string Content { get; set; }
        [Required]
        public int Status { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateComplete { get; set; }
        public DateTime DateLine { get; set; }

    }
}
