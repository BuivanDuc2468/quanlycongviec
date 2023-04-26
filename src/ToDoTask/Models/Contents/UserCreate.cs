using System.ComponentModel.DataAnnotations;

namespace ToDoTask.Models.Contents
{
    public class UserCreate
    {
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; }
        [Display(Name = "Họ Tên")]
        public string Name { get; set; }
        [Display(Name = "Địa Chỉ Email")]
        public string Email { get; set; }
        [Display(Name = "Mật Khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Quyền")]
        public string Role { get; set; }

    }
}
