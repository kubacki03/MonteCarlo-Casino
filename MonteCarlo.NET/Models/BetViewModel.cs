namespace MonteCarlo.NET.Models
{
    public class BetViewModel
    {
        public string SelectedHorse { get; set; }
        public decimal Amount { get; set; }

        public List<Horse> AvailableHorses { get; set; }

        public bool IsRaceFinished { get; set; }
        public string Winner { get; set; }
        public bool IsWinner { get; set; }
        public decimal WinAmount { get; set; }
        public List<HorseResult> HorseResults { get; set; }
    }
    public class HorseResult
    {
        public string HorseName { get; set; }
        public float Time { get; set; }
        public string Color { get; set; }
    }


}

