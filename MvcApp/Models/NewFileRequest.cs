using System.ComponentModel.DataAnnotations;

namespace MvcApp.Models
{
    public class NewFileRequest
    {
        [Required]
        [Display(Name = "File name")]
        public string FileName { get; set; }

        [Display(Name = "File contents")]
        public string Content { get; set; }
    }
}