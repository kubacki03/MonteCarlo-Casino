namespace MonteCarlo.NET.Models
{
    public class Horse
    {
        public int Age { get; set; }
        public float Weight { get; set; }
        public float Size { get; set; }
        public string Color { get; set; }
        public string Name { get; set; }

        public float Speed { get; set; }
        public float Stamina { get; set; }

        public float WinRatio { get; set; }
        public int WinCount { get; set; }

        private float DistanceCovered { get; set; }
    }
}
