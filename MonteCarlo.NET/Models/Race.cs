namespace MonteCarlo.NET.Models
{
    public class Race
    {
        public int Number { get; set; }
        public const int TrackLength = 300;
        public List<Horse> Horses { get; set; }

        public List<float> Times { get; set; }

        public Race(List<Horse> horses)
        {
            Horses = horses;
            Times = new List<float>();
        }

        public Horse RunRace()
        {
            Horse winner = null;

            if (Horses == null)
            {
                throw new InvalidOperationException("Brak koni na liście");
            }

            Random rand = new Random();
            foreach (Horse horse in Horses)
            {
                float time = 0;
                int remainingDistance = TrackLength;
                float stamina = horse.Stamina;
                float speed = horse.Speed;

                float luck = 1.0f + (float)(rand.NextDouble() * 0.2 - 0.1);

                while (remainingDistance > 0)
                {
                    float min = 0.02f;
                    float max = 0.08f;

                    float randomEvent = (float)(rand.NextDouble());
                    if (randomEvent < 0.1)
                    {
                        stamina -= 0.15f;
                    }
                    else if (randomEvent > 0.9)
                    {
                        speed += 0.2f * speed;
                    }

                    stamina -= (float)(rand.NextDouble() * (max - min) + min);
                    if (stamina <= 0.2f)
                    {
                        stamina = 0.2f;
                    }

                    speed = horse.Speed * stamina * luck;

                    float segmentTime = 100 / speed;
                    time += segmentTime;
                    remainingDistance -= 100;
                }

                Times.Add(time);
            }


            var horsesWithTimes = Horses.Zip(Times, (horse, time) => new { Horse = horse, Time = time })
                                     .OrderBy(pair => pair.Time)
                                     .ToList();
            if (horsesWithTimes.Count > 0)
            {
                horsesWithTimes[0].Horse.WinCount++;
                winner = horsesWithTimes[0].Horse;
            }

            return winner;

        }
    }
}
