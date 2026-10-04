using System.ComponentModel.DataAnnotations;

namespace MonteCarlo.NET.Models
{
    public class GraKonto
    {
        [Required]
        public int IdGraKonto { get; set; }
        [Required]
        public string KontoUzytkownikaId { get; set; }
        public virtual KontoUzytkownika KontoUzytkownika { get; set; }
        [Required]
        public double IleWygrano { get; set; }
        [Required]
        public double IlePostawiono { get; set; }
        public DateTime Czas { get; set; } = DateTime.Now;//jeśli chcemy dodawać date kiedy grano w grę wystarczy odkomentować
        [Required]
        public int IdGry { get; set; }
        public virtual Gra Gra { get; set; }
    }
}
