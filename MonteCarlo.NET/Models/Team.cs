using Microsoft.Build.Framework;

namespace MonteCarlo.NET.Models
{
    public class Druzyna
    {
        [Required]
        public int DruzynaId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string League { get; set; }

    }
}
