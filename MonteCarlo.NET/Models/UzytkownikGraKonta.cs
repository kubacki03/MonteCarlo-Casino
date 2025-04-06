namespace MonteCarlo.NET.Models
{
    public class UzytkownikGraKonta
    {
        public string NazwaUzytkownika { get; set; }
        public string Nazwisko { get; set; }
        public string Imie { get; set; }
        public string NazwaGry { get; set; }
        public float IlePostawiono { get; set; }
        public float IleWygrano { get; set; }
        public DateTime Czas { get; set; }

    }
}
