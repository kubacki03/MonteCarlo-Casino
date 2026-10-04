 using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;

namespace MonteCarlo.NET.Models
{
	public class UserAccount : IdentityUser
	{
		[Required]
		[MinLength(2)]
		[MaxLength(50)]
		[Column("Imie")]
		public string FirstName { get; set; }
		[Required]
		[MinLength(2)]
		[MaxLength(50)]
		[Column("Nazwisko")]
		public string LastName { get; set; }
		[Column("Saldo")]
		public double? Balance { get; set; }
		public int? Level { get; set; }
		public virtual ICollection<GameAccount>? GameAccounts { get; set; }
		public virtual ICollection<Transaction>? Transactions { get; set; }
		public virtual ICollection<Ban>? Bans { get; set; }
		public virtual ICollection<Report>? Reports { get; set; }
		public virtual ICollection<Limit>? Limits { get; set; }
	}
}
