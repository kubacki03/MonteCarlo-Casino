using System.ComponentModel.DataAnnotations;

namespace MonteCarlo.NET.Models
{
	public class Gra
	{
		[Required]
		public int IdGry { get; set; }
		[Required]
		[MaxLength(150)]
		[MinLength(2)]
		public string Nazwa { get; set; }
		[Required]
		public double MinStawka { get; set; }
		public virtual ICollection<GraKonto>? GryKonta { get; set; }
	}
}
