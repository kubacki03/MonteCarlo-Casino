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
}
