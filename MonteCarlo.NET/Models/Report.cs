using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;

namespace MonteCarlo.NET.Models
{
	public class Report
	{
		[Required]
		[Column("IdZgloszenia")]
		public int ReportId { get; set; }
		[Required]
		[Column("KontoUzytkownikaId")]
		public string UserAccountId { get; set; }
		public virtual UserAccount UserAccount { get; set; }
		[Required]
		[MaxLength(50)]
		[Column("Tytul")]
		public string Title { get; set; }
		[Required]
		[MaxLength(250)]
		[Column("Tresc")]
		public string Content { get; set; }
		[Required]
		[Column("Data")]
		public DateTime Date { get; set; }
		[Required]
		[MaxLength(50)]
		public string Status { get; set; }
		[MaxLength(250)]
		[Column("Notatki")]
		public string? Notes { get; set; }
	}
}
