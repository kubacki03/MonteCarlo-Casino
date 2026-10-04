namespace MonteCarlo.NET.Models
{
    public class CloverScratchCard
    {
        public string[] Fields { get; set; }
        public int WinningCombinationCount { get; set; }
        public bool IsGameOver { get; set; }

        public int Prize { get; set; }


        public CloverScratchCard()
        {
            Fields = new string[9];
            IsGameOver = false;
        }

        public void InitGame(Random random)
        {
            int cloverCount = 0;
            for (int i = 0; i < Fields.Length; i++)
            {
                var chance = random.Next(0, 10);
                Fields[i] = chance < 1 ? "clover" : "empty";
                if (chance < 1)
                {
                    cloverCount++;
                }
            }
            WinningCombinationCount = Fields.Count(f => f == "clover");

            if (cloverCount == 0)
            {
                Prize = 20;
            }
            else
            {
                Prize = random.Next(1, 10);
            }
        }

        public bool CheckForWin()
        {
            return WinningCombinationCount >= 3;
        }

        public void ResetGame(Random random)
        {
            InitGame(random);
            IsGameOver = false;
        }
    }
}