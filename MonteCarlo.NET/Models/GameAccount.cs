using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;

namespace MonteCarlo.NET.Models
{
    public class GameAccount
    {
        [Required]
        [Column("IdGraKonto")]
        public int GameAccountId { get; set; }
        [Required]
        [Column("KontoUzytkownikaId")]
        public string UserAccountId { get; set; }
        public virtual UserAccount UserAccount { get; set; }
        [Required]
        [Column("IleWygrano")]
        public double AmountWon { get; set; }
        [Required]
        [Column("IlePostawiono")]
        public double AmountStaked { get; set; }
        [Column("Czas")]
        public DateTime PlayedAt { get; set; } = DateTime.Now;
        [Required]
        [Column("IdGry")]
        public int GameId { get; set; }
        public virtual Game Game { get; set; }
    }
}
