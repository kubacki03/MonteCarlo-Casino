using Microsoft.AspNetCore.Routing.Constraints;
using MonteCarlo.NET.Controllers;

namespace MonteCarlo.NET.Models
{
    public class RouletteNumber
    {
        public int Number { get; set; }
        public string Color { get; set; } // "Red", "Black", or "Green" for 0/00
        public string Row { get; set; }  // "1st 12", "2nd 12", "3rd 12"

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


    public class RuletkaFunkcje
    {
        // 0-36 = numerki
        // 37 = czarne
        // 38 = czerwone
        // 39-41 = rzędy (odpowiednio 39/ dzielenie przez 3 40/ dzielenie przez 3 daje reszte 1 etc.)
        // 42-44 = kolumnny (odpowiednio 42 od 1 do 12/ 43 od 13 do 24 etc.)
        // 45 = 1-18
        // 46 = 19-36
        // 47 = parzyste
        // 48 = nieparzyste
        public static float[] bets = new float[49];

        public static string[] betString =
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

        public static int wynik;

        public static int spin()
        {
            RandomNumberGenerator rng = new RandomNumberGenerator();

            int wynik = (int)rng.Next(37);

            return wynik;
        }


        public static void bet_numbers(float money, int position)
        {
            if (money > 0 && money == Math.Floor(money))
            {
                bets[position] = money;
                //zabierz kasę z konta
            }
        }

        public static void clear_bets()
        {
            for (int i = 0; i < bets.Length; i++)
            {
                bets[i] = 0;
            }
        }

        public static float checkWin(int mode, float money)
        {
            int[] czerwone = { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 };

            if (mode <= 36) //numerki
            {
                if (mode == wynik)
                {
                    return 35 * money;
                }
            }
            else if (mode == 37) //czarne
            {
                if (!czerwone.Contains(wynik) && wynik != 0)
                {
                    return 2 * money;
                }
            }
            else if (mode == 38) //czerwone
            {
                if (czerwone.Contains(wynik) && wynik != 0)
                {
                    return 2 * money;
                }
            }
            else if (mode <= 41) //rzędy
            {
                float rzad = wynik / 12;
                if (Math.Ceiling(rzad) == mode - 38)
                {
                    return 3 * money;
                }
            }
            else if (mode <= 44) //kolumny
            {
                int kolumna = (wynik - 1) % 3;
                if (kolumna == mode - 42 && wynik != 0)
                {
                    return 3 * money;
                }
            }
            else if (mode <= 46) //1-18 i 19-36
            {
                float rzad = wynik / 18;
                if (Math.Ceiling(rzad) == mode - 44)
                {
                    return 2 * money;
                }
            }
            else if (mode <= 48) //parzyste/nieparzyste
            {
                if (wynik % 2 == mode - 47 && wynik != 0)
                {
                    return 2 * money;
                }
            }

            return 0;
        }

        public static float grac()
        {
            wynik = spin();
            float suma = 0;

            for (int i = 0; i < bets.Length; i++)
            {
                if (bets[i] > 0)
                {
                    suma += checkWin(i, bets[i]);
                }
            }

            return suma;
        }

        public static float gracDwa()
        {
            float suma = 0;

            for (int i = 0; i < bets.Length; i++)
            {
                suma += bets[i];
            }

            clear_bets();

            return suma;
        }
    }

    class RandomNumberGenerator
    {
        private const long m = 4294967296; // = 2^32
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
