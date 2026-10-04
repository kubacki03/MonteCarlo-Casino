using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;

namespace MonteCarlo.NET.Models
{
    public class Bet
    {
        [Key]
        [Column("IdZakladu")]
        public int BetId { get; set; }

        [Required]
        [Column("IdGracza")]
        public string PlayerId { get; set; }

        [Required]
        [Column("IdMeczu")]
        public int MatchId { get; set; }

        [Required]
        [Column("IdZwyciezcy")]
        public int WinnerTeamId { get; set; }

        [Required]
        [Column("PostawionaKwota")]
        public long StakedAmount { get; set; }

        [Required]
        [Column("czyPrzyznanoNagrode")]
        public bool IsRewardGranted { get; set; }

       
        [Column("czyWygral")]
        public bool HasWon { get; set; }

    }
}
