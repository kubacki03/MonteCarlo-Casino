namespace MonteCarlo.NET.Models
{
    public class Wyscig
    {
        public int numer { get; set; }
        public const int trasa = 300;
        public List<Konie> Lista_Koni { get; set; }
        //List<Konie> Lista_Koni = new List<Konie>();

        public List<float> Czas { get; set; }

        public Wyscig(List<Konie> konie)
        {
            Lista_Koni = konie;
            Czas = new List<float>();
        }

        public Konie RozegrajWyscig()
        {
            Konie zwyciezca = null;

            if (Lista_Koni == null)
            {
                throw new InvalidOperationException("Brak koni na liście");
            }

            Random rand = new Random();
            /*foreach (Konie kon in Lista_Koni)
            {
                float czas = 0;
                int pozostalaTrasa = trasa;
                float wytrzymalosc = kon.wytrzymalosc;
                float predkosc = kon.predkosc;

                while (pozostalaTrasa > 0)
                {
                    float min = 0.01f;
                    float max = 0.1f;


                    wytrzymalosc -= (float)(rand.NextDouble() * (max - min) + min);
                    if (wytrzymalosc <= 0)
                    {
                        wytrzymalosc = 0.1f;
                    }
                    predkosc = predkosc * wytrzymalosc;
                    float odcinekCzasu = 100 / predkosc;
                    czas += odcinekCzasu;
                    pozostalaTrasa -= 100;


                }

                Czas.Add(czas);
            }*/
            foreach (Konie kon in Lista_Koni)
            {
                float czas = 0;
                int pozostalaTrasa = trasa;
                float wytrzymalosc = kon.wytrzymalosc;
                float predkosc = kon.predkosc;

                // Losowy współczynnik szczęścia (wpływa na ostateczne wyniki)
                float szczescie = 1.0f + (float)(rand.NextDouble() * 0.2 - 0.1); // od 0.9 do 1.1

                while (pozostalaTrasa > 0)
                {
                    float min = 0.02f; // Minimalny spadek wytrzymałości
                    float max = 0.08f; // Maksymalny spadek wytrzymałości

                    // Losowe zdarzenie (przyspieszenie lub spadek wytrzymałości)
                    float zdarzenie = (float)(rand.NextDouble());
                    if (zdarzenie < 0.1) // 10% szans na dodatkowy spadek wytrzymałości
                    {
                        wytrzymalosc -= 0.15f;
                    }
                    else if (zdarzenie > 0.9) // 10% szans na przyspieszenie
                    {
                        predkosc += 0.2f * predkosc; // 20% wzrost prędkości
                    }

                    // Regularny spadek wytrzymałości
                    wytrzymalosc -= (float)(rand.NextDouble() * (max - min) + min);
                    if (wytrzymalosc <= 0.2f) // Minimalna wytrzymałość
                    {
                        wytrzymalosc = 0.2f;
                    }

                    // Dynamiczne dostosowanie prędkości
                    predkosc = kon.predkosc * wytrzymalosc * szczescie;

                    // Obliczenie czasu dla odcinka
                    float odcinekCzasu = 100 / predkosc;
                    czas += odcinekCzasu;
                    pozostalaTrasa -= 100;
                }

                Czas.Add(czas);
            }


            var konieZCzasem = Lista_Koni.Zip(Czas, (kon, czas) => new { Kon = kon, Czas = czas })
                                     .OrderBy(pair => pair.Czas)
                                     .ToList();
            if (konieZCzasem.Count > 0)
            {
                konieZCzasem[0].Kon.ilosc_zwyciestw++;
                zwyciezca = konieZCzasem[0].Kon;
            }

            return zwyciezca;

        }
    }
}
