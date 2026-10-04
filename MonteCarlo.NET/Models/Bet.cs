using System.ComponentModel.DataAnnotations;

namespace MonteCarlo.NET.Models
{
    public class Zaklad
    {
        [Key]
        public int IdZakladu { get; set; }

        [Required]
        public string IdGracza { get; set; }

        [Required]
        public int IdMeczu { get; set; }

        [Required]
        public int IdZwyciezcy { get; set; }

        [Required]
        public long PostawionaKwota { get; set; }

        [Required]
        public bool czyPrzyznanoNagrode { get; set; }

       
        public bool czyWygral { get; set; }

    }
}
