using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Data.Entities
{
    [Table("ProjectUsers")]
    public class ProjectUser
    {
        [Key]
        [Required]
        [Column(TypeName = "varchar(50)")]
        public string Id { get; set; }
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        [Required]
        public string UserId { get; set; }
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        [Required]
        public string IdManager { get; set; }
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        [Required]
        public string ProjectId { get; set; }
    }
}
