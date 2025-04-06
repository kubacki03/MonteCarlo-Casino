using System.ComponentModel.DataAnnotations;

namespace MonteCarlo.NET.Models
{
    public class FormularzZgloszenie
    {
        [Required]
        [MaxLength(50)]
        public string Tytul { get; set; }
        [Required]
        [MaxLength(250)]
        public string Tresc { get; set; }
        [MaxLength(250)]
        public string? Notatki { get; set; }
    }
}
