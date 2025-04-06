namespace MonteCarlo.NET.Models
{
	public class UzytkownikBan
	{
        public string NazwaUzytkownika { get; set; }
        public string Nazwisko { get; set; }
        public string Imie { get; set; }
        public DateTime Data { get; set; }
        public int Dlugosc { get; set; }
        public string Przyczyna { get; set; }
    }
}
