namespace MonteCarlo.NET.Models
{
	public class UzytkownikZgloszenie
	{
        public int IdZgloszenia { get; set; }
        public string NazwaUzytkownika { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Status { get; set; }
        public DateTime Data { get; set; }
        public string Tytul { get; set; }
    }
}
