using System.ComponentModel.DataAnnotations;

namespace MonteCarlo.NET.Models
{
	public class Ban
	{
		[Required]
		public int IdBana { get; set; }
		[Required]
		[DataType(DataType.Date)]
		public DateTime Data { get; set; } = DateTime.Today;
		[Required]
		public int Dlugosc { get; set; }
		[MaxLength(250)]
		public string? Przyczyna { get; set; }
		[Required]
		public string KontoUzytkownikaId { get; set; }
		public virtual KontoUzytkownika KontoUzytkownika { get; set; }
	}
}
