namespace MonteCarlo.NET.Models
{
    public class Konie
    {
        public int wiek { get; set; }
        public float waga { get; set; }
        public float rozmiar { get; set; }
        public string kolor { get; set; }
        public string imie { get; set; }

        public float predkosc { get; set; }
        public float wytrzymalosc { get; set; }

        public float winratio { get; set; }
        public int ilosc_zwyciestw { get; set; }

        private float pokonana_trasa { get; set; }
    }
}
