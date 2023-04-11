using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace ToDoList.Data.Entities
{
    [Table("Projects")]
    public class Project
    {
        [Key]
        [Column(TypeName = "varchar(50)")]
        public string Id { get; set; }
        [Required]
        [Column(TypeName = "varchar(200)")]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        [Required]
        public string UserId { get; set; }
    }
}
