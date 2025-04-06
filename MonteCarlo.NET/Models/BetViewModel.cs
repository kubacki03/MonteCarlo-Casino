namespace MonteCarlo.NET.Models
{
    public class BetViewModel
    {
        public string WybranyKon { get; set; }
        public decimal Kwota { get; set; }

        public List<Konie> DostepneKonie { get; set; }

        // Ewentualnie pola do wyświetlenia wyników po wyścigu
        public bool WyscigRozstrzygniety { get; set; }
        public string Zwyciezca { get; set; }
        public bool Wygrana { get; set; }
        public decimal WygranaKwota { get; set; }
        public List<HorseResult> WynikiKoni { get; set; }
    }
    public class HorseResult
    {
        public string KonImie { get; set; }
        public float Czas { get; set; }
        public string Kolor { get; set; }
    }


}

