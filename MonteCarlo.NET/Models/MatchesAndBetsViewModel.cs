namespace MonteCarlo.NET.Models
{
    public class MatchesAndBetsViewModel
    {
      
        public IEnumerable<Mecz> Matches { get; set; }
        public IEnumerable<Zaklad> Bets { get; set; }
    }

}
