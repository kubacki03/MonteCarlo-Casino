using System.ComponentModel.DataAnnotations;

namespace MonteCarlo.NET.Models
{
    public class Transakcja
    {
        [Required]
        public int IdTransakcji { get; set; }
        [Required]
        public string KontoUzytkownikaId { get; set; }
        public virtual KontoUzytkownika KontoUzytkownika { get; set; }
        [Required]
        public DateTime Data { get; set; }
        [Required]
        public double Kwota { get; set; }
        [Required]
        [MaxLength(50)]
        public string Typ { get; set; }
    }
}
