using Microsoft.Build.Framework;

using System.ComponentModel.DataAnnotations.Schema;

namespace MonteCarlo.NET.Models
{
    public class Team
    {
        [Required]
        [Column("DruzynaId")]
        public int TeamId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string League { get; set; }

    }
}
