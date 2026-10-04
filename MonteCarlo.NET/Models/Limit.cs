using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;

namespace MonteCarlo.NET.Models
{
	public class Limit
	{
		[Required]
		[Column("IdLimitu")]
		public int LimitId { get; set; }
		[Required]
		[Column("KontoUzytkownikaId")]
		public string UserAccountId { get; set; }
		public virtual UserAccount UserAccount { get; set; }
		[Required]
		[Column("Kwota")]
		public double Amount { get; set; }
		[Required]
		[Column("Data")]
		public DateTime Date { get; set; }

	}
}
