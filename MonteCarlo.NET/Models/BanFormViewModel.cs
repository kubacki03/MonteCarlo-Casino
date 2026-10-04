using System.ComponentModel.DataAnnotations;

namespace MonteCarlo.NET.Models
{
	public class FormularzBan
	{
		[Required]
		[DataType(DataType.Date)]
		public DateTime Data { get; set; } = DateTime.Today;
		[Required]
		public int Dlugosc { get; set; }
		[Required]
		[MaxLength(250)]
		public string Przyczyna { get; set; }
	}
}
