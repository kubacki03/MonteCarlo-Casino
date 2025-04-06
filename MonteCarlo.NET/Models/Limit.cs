using System.ComponentModel.DataAnnotations;

namespace MonteCarlo.NET.Models
{
	public class Limit
	{
		[Required]
		public int IdLimitu { get; set; }
		[Required]
		public string KontoUzytkownikaId { get; set; }
		public virtual KontoUzytkownika KontoUzytkownika { get; set; }
		[Required]
		public double Kwota { get; set; }
		[Required]
		public DateTime Data { get; set; }

	}
}
