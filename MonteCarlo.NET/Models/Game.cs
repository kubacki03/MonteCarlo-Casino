using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;

namespace MonteCarlo.NET.Models
{
	public class Game
	{
		[Required]
		[Column("IdGry")]
		public int GameId { get; set; }
		[Required]
		[MaxLength(150)]
		[MinLength(2)]
		[Column("Nazwa")]
		public string Name { get; set; }
		[Required]
		[Column("MinStawka")]
		public double MinStake { get; set; }
		public virtual ICollection<GameAccount>? GameAccounts { get; set; }
	}
}
