 using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MonteCarlo.NET.Models
{
	public class KontoUzytkownika : IdentityUser
	{
		//[Required]
		//public int NrKonta { get; set; }
		[Required]
		[MinLength(2)]
		[MaxLength(50)]
		public string Imie { get; set; }
		[Required]
		[MinLength(2)]
		[MaxLength(50)]
		public string Nazwisko { get; set; }
		//[Required]
		//[MaxLength(50)]
		//public string Email { get; set; }
		//[Required]
		//[MaxLength(50)]
		//public string Login { get; set; }
		//[Required]
		//[MaxLength(50)]
		//public string Password { get; set; }
		public double? Saldo { get; set; }
		//[Required]
		//public string Status { get; set; } // status przyjmuje wartości: standardowy, VIP, admin
		public int? Level { get; set; }
		public virtual ICollection<GraKonto>? GryKonta { get; set; }
		public virtual ICollection<Transakcja>? Trasakcje { get; set; }
		public virtual ICollection<Ban>? Bany { get; set; }
		public virtual ICollection<Zgloszenie>? Zgloszenia { get; set; }
		public virtual ICollection<Limit>? Limity { get; set; }
	}
}
