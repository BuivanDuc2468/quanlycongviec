using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ToDoTask.Models.Contents
{
    public class UserVm
    {
        public string NameUser { get; set; }
        public string UserId { get; set; }
    }
}
