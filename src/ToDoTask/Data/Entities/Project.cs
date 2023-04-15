using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace ToDoTask.Data.Entities
{
    [Table("Projects")]
    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [MaxLength(50)]
        public string Id { get; set; }
        [Required]
        [Column(TypeName = "varchar(200)")]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime DateLine { get; set; }
        [Required]
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string UserId { get; set; }
    }
}
