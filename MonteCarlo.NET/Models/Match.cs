using System.ComponentModel.DataAnnotations;

namespace MonteCarlo.NET.Models
{
    public class Mecz
    {
        [Required]
        public int MeczId { get; set; }

        [Required]
        public DateOnly data { get; set; }

        [Required]
        public int HomeTeamId { get; set; }  // Change long to int
        public virtual Druzyna DruzynaHome { get; set; }

        [Required]
        public string HomeTeamName { get; set; }

        [Required]
        public string AwayTeamName { get; set; }

        [Required]
        public int AwayTeamId { get; set; }  // Change long to int
        public virtual Druzyna DruzynaAway { get; set; }

        
        public int HomeTeamGoals { get; set; }

        
        public int AwayTeamGoals { get; set; }

        [Required]
        public float HomeTeamOdds { get; set; }

        [Required]
        public float AwayTeamOdds { get; set; }
    }

}
