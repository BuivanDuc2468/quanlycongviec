using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace ToDoTask.Data.Entities
{
    [Table("Projects")]
    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(300)")]
        public string Name { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Trạng Thái")]
        public int Status { get; set; }
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; }
        [Display(Name = "Hạn Cuối")]
        public DateTime DateLine { get; set; }
        [Required]
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<ProjectUser> ProjectUsers { get; set; }
    }
}
