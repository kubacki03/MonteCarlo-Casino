using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;

namespace MonteCarlo.NET.Models
{
	public class Ban
	{
		[Required]
		[Column("IdBana")]
		public int BanId { get; set; }
		[Required]
		[DataType(DataType.Date)]
		[Column("Data")]
		public DateTime Date { get; set; } = DateTime.Today;
		[Required]
		[Column("Dlugosc")]
		public int DurationDays { get; set; }
		[MaxLength(250)]
		[Column("Przyczyna")]
		public string? Reason { get; set; }
		[Required]
		[Column("KontoUzytkownikaId")]
		public string UserAccountId { get; set; }
		public virtual UserAccount UserAccount { get; set; }
	}
}
