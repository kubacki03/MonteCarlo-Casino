using Microsoft.AspNetCore.Routing.Constraints;
using MonteCarlo.NET.Controllers;

namespace MonteCarlo.NET.Models
{
    public class RouletteNumber
    {
        public int Number { get; set; }
        public string Color { get; set; }
        public string Row { get; set; }

        public static List<RouletteNumber> GenerateRouletteNumbers()
        {
            return new List<RouletteNumber>
            {
                new RouletteNumber { Number = 3, Color = "Red", Row = "1st 12" },
                new RouletteNumber { Number = 6, Color = "Black", Row = "1st 12" },
                new RouletteNumber { Number = 9, Color = "Red", Row = "1st 12" },
                new RouletteNumber { Number = 12, Color = "Red", Row = "1st 12" },
                new RouletteNumber { Number = 15, Color = "Black", Row = "2nd 12" },
                new RouletteNumber { Number = 18, Color = "Red", Row = "2nd 12" },
                new RouletteNumber { Number = 21, Color = "Red", Row = "2nd 12" },
                new RouletteNumber { Number = 24, Color = "Black", Row = "2nd 12" },
                new RouletteNumber { Number = 27, Color = "Red", Row = "3rd 12" },
                new RouletteNumber { Number = 30, Color = "Black", Row = "3rd 12" },
                new RouletteNumber { Number = 33, Color = "Red", Row = "3rd 12" },
                new RouletteNumber { Number = 36, Color = "Black", Row = "3rd 12" },

                new RouletteNumber { Number = 2, Color = "Black", Row = "1st 12" },
                new RouletteNumber { Number = 5, Color = "Red", Row = "1st 12" },
                new RouletteNumber { Number = 8, Color = "Black", Row = "1st 12" },
                new RouletteNumber { Number = 11, Color = "Black", Row = "1st 12" },
                new RouletteNumber { Number = 14, Color = "Red", Row = "2nd 12" },
                new RouletteNumber { Number = 17, Color = "Black", Row = "2nd 12" },
                new RouletteNumber { Number = 20, Color = "Black", Row = "2nd 12" },
                new RouletteNumber { Number = 23, Color = "Red", Row = "2nd 12" },
                new RouletteNumber { Number = 26, Color = "Black", Row = "2nd 12" },
                new RouletteNumber { Number = 29, Color = "Red", Row = "3rd 12" },
                new RouletteNumber { Number = 32, Color = "Black", Row = "3rd 12" },
                new RouletteNumber { Number = 35, Color = "Red", Row = "3rd 12" },

                new RouletteNumber { Number = 1, Color = "Red", Row = "1st 12" },
                new RouletteNumber { Number = 4, Color = "Black", Row = "1st 12" },
                new RouletteNumber { Number = 7, Color = "Red", Row = "1st 12" },
                new RouletteNumber { Number = 10, Color = "Black", Row = "1st 12" },
                new RouletteNumber { Number = 13, Color = "Black", Row = "2nd 12" },
                new RouletteNumber { Number = 16, Color = "Red", Row = "2nd 12" },
                new RouletteNumber { Number = 19, Color = "Red", Row = "2nd 12" },
                new RouletteNumber { Number = 22, Color = "Black", Row = "2nd 12" },
                new RouletteNumber { Number = 25, Color = "Red", Row = "2nd 12" },
                new RouletteNumber { Number = 28, Color = "Black", Row = "3rd 12" },
                new RouletteNumber { Number = 31, Color = "Red", Row = "3rd 12" },
                new RouletteNumber { Number = 34, Color = "Black", Row = "3rd 12" },

                new RouletteNumber { Number = 0, Color = "Green", Row = "" }
            };
        }
    }


    public class RouletteLogic
    {
        public static float[] Bets = new float[49];

        public static string[] BetLabels =
        {
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
            "11", "12", "13", "14", "15", "16", "17", "18", "19", "20",
            "21", "22", "23", "24", "25", "26", "27", "28", "29", "30",
            "31", "32", "33", "34", "35", "36",
            "Czarne", "Czerwone",
            "Rzad 1", "Rzad 2", "Rzad 3",
            "Kolumna 1", "Kolumna 2", "Kolumna 3",
            "1-18", "19-36",
            "Parzyste", "Nieparzyste",
        };

        public static int LastResult;

        public static int Spin()
        {
            RandomNumberGenerator rng = new RandomNumberGenerator();

            int result = (int)rng.Next(37);

            return result;
        }


        public static void PlaceBet(float money, int position)
        {
            if (money > 0 && money == Math.Floor(money))
            {
                Bets[position] = money;
            }
        }

        public static void ClearBets()
        {
            for (int i = 0; i < Bets.Length; i++)
            {
                Bets[i] = 0;
            }
        }

        public static float CheckWin(int mode, float money)
        {
            int[] redNumbers = { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 };

            if (mode <= 36)
            {
                if (mode == LastResult)
                {
                    return 35 * money;
                }
            }
            else if (mode == 37)
            {
                if (!redNumbers.Contains(LastResult) && LastResult != 0)
                {
                    return 2 * money;
                }
            }
            else if (mode == 38)
            {
                if (redNumbers.Contains(LastResult) && LastResult != 0)
                {
                    return 2 * money;
                }
            }
            else if (mode <= 41)
            {
                float row = LastResult / 12;
                if (Math.Ceiling(row) == mode - 38)
                {
                    return 3 * money;
                }
            }
            else if (mode <= 44)
            {
                int column = (LastResult - 1) % 3;
                if (column == mode - 42 && LastResult != 0)
                {
                    return 3 * money;
                }
            }
            else if (mode <= 46)
            {
                float row = LastResult / 18;
                if (Math.Ceiling(row) == mode - 44)
                {
                    return 2 * money;
                }
            }
            else if (mode <= 48)
            {
                if (LastResult % 2 == mode - 47 && LastResult != 0)
                {
                    return 2 * money;
                }
            }

            return 0;
        }

        public static float SpinAndCalculateWinnings()
        {
            LastResult = Spin();
            float sum = 0;

            for (int i = 0; i < Bets.Length; i++)
            {
                if (Bets[i] > 0)
                {
                    sum += CheckWin(i, Bets[i]);
                }
            }

            return sum;
        }

        public static float CollectTotalBets()
        {
            float sum = 0;

            for (int i = 0; i < Bets.Length; i++)
            {
                sum += Bets[i];
            }

            ClearBets();

            return sum;
        }
    }

    class RandomNumberGenerator
    {
        private const long m = 4294967296;
        private const long a = 1664525;
        private const long c = 1013904223;
        private long _last;

        public RandomNumberGenerator()
        {
            _last = DateTime.Now.Ticks % m;
        }

        public RandomNumberGenerator(long seed)
        {
            _last = seed;
        }

        public long Next()
        {
            _last = ((a * _last) + c) % m;

            return _last;
        }

        public long Next(long maxValue)
        {
            return Next() % maxValue;
        }
    }

}
