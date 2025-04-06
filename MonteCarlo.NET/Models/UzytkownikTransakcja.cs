namespace MonteCarlo.NET.Models
{
	public class UzytkownikTransakcja
	{
        public string NazwaUzytkownika { get; set; }
        public string Nazwisko { get; set; }
        public string Imie { get; set; }
        public DateTime Data { get; set; }
        public float Kwota { get; set; }
        public string Typ { get; set; }
    }
}
