using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ToDoTask.Data.Entities;

namespace ToDoTask.Models.Contents
{
    public class JobForView : Job
    {
        [Display(Name = "Aassigned person")]
        public string UserName { get; set; }
        public string UserId { get; set; }
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }
        public DateTime DateAssign { get; set; }
    } 
}
