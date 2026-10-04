using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;

namespace MonteCarlo.NET.Models
{
    public class Match
    {
        [Required]
        [Column("MeczId")]
        public int MatchId { get; set; }

        [Required]
        [Column("data")]
        public DateOnly Date { get; set; }

        [Required]
        public int HomeTeamId { get; set; }
        public virtual Team HomeTeam { get; set; }

        [Required]
        public string HomeTeamName { get; set; }

        [Required]
        public string AwayTeamName { get; set; }

        [Required]
        public int AwayTeamId { get; set; }
        public virtual Team AwayTeam { get; set; }

        
        public int HomeTeamGoals { get; set; }

        
        public int AwayTeamGoals { get; set; }

        [Required]
        public float HomeTeamOdds { get; set; }

        [Required]
        public float AwayTeamOdds { get; set; }
    }

}
