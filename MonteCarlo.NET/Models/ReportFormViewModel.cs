using System.ComponentModel.DataAnnotations;

namespace MonteCarlo.NET.Models
{
    public class ReportFormViewModel
    {
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }
        [Required]
        [MaxLength(250)]
        public string Content { get; set; }
        [MaxLength(250)]
        public string? Notes { get; set; }
    }
}
