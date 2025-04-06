namespace MonteCarlo.NET.Models
{
    public class ZdrapkaKoniczynka
    {
        public string[] Fields { get; set; }
        public int WinningCombinationCount { get; set; } // Liczba koniczynek
        public bool IsGameOver { get; set; }

        public int Wygrana { get; set; }


        public ZdrapkaKoniczynka()
        {
            Fields = new string[9];
            IsGameOver = false;
        }

        public void InitGame()
        {
            int ile = 0;
            var random = new Random();
            for (int i = 0; i < Fields.Length; i++)
            {
                var szansa = random.Next(0, 10);
                Fields[i] = szansa < 1 ? "clover" : "empty"; // 20% szans na koniczynkę
                if (szansa < 1)
                {
                    ile++;
                }
            }
            WinningCombinationCount = Fields.Count(f => f == "clover");

            if (ile == 0)
            {
                Wygrana = 20;
            }
            else
            {
                Wygrana = random.Next(1, 10);
            }
        }

        public bool CheckForWin()
        {
            return WinningCombinationCount >= 3; // Sprawdzamy, czy mamy 3 koniczynki
        }

        public void ResetGame()
        {
            InitGame();
            IsGameOver = false;
        }
    }
}