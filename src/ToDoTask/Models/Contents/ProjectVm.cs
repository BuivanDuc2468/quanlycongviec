using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ToDoTask.Models.Contents
{
    public class ProjectVm
    {
        public string Id { get; set; }
        [Display(Name = "Tên dự án")]
        public string Name { get; set; }
        [Display(Name = "Mô tả")]
        public string Description { get; set; }
        public int Status { get; set; }
        [Display(Name = "Ngày Tạo")]
        public DateTime CreatedAt { get; set; }
        [Display(Name = "Người Tạo")]
        public string UserId { get; set; }
        [Display(Name = "Hạn kết thúc")]
        public DateTime DateLine { get; set; }
    }
}
