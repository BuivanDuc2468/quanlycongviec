using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ToDoTask.Data.Entities
{
    [Table("Jobs")]
    public class Job
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Dự Án")]
        public string ProjectId { get; set; }
        [Required]
        [Display(Name = "Nhân Viên")]
        public string UserId { get; set; }
        [MaxLength(200)]
        [Required]
        [Display(Name = "Tên Công Việc")]
        public string Name { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]
        [Display(Name = "Nội dung")]
        public string Content { get; set; }
        [Required]
        [Display(Name = "Trạng Thái")]
        public int Status { get; set; }
        [Display(Name = "Ngày bắt đầu")]
        public DateTime? DateStart { get; set; }
        [Display(Name = "Ngày hoàn thành")]
        public DateTime? DateComplete { get; set; }
        [Display(Name = "Hạn cuối")]
        public DateTime DateLine { get; set; }
        [Display(Name = "Ngày được giao")]
        public DateTime DateAssign { get; set; }
        public virtual ProjectUser ProjectUser { get; set; }
    }
}
