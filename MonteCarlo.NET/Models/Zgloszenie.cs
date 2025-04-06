using System.ComponentModel.DataAnnotations;

namespace MonteCarlo.NET.Models
{
	public class Zgloszenie
	{
		[Required]
		public int IdZgloszenia { get; set; }
		[Required]
		public string KontoUzytkownikaId { get; set; }
		public virtual KontoUzytkownika KontoUzytkownika { get; set; }
		[Required]
		[MaxLength(50)]
		public string Tytul { get; set; }
		[Required]
		[MaxLength(250)]
		public string Tresc { get; set; }
		[Required]
		public DateTime Data { get; set; }
		[Required]
		[MaxLength(50)]
		public string Status { get; set; }
		[MaxLength(250)]
		public string? Notatki { get; set; }
	}
}
