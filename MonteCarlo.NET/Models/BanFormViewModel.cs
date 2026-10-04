using System.ComponentModel.DataAnnotations;

namespace MonteCarlo.NET.Models
{
	public class BanFormViewModel
	{
		[Required]
		[DataType(DataType.Date)]
		public DateTime Date { get; set; } = DateTime.Today;
		[Required]
		public int DurationDays { get; set; }
		[Required]
		[MaxLength(250)]
		public string Reason { get; set; }
	}
}
