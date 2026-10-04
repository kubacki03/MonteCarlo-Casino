namespace MonteCarlo.NET.Models
{
    public class UserGameAccount
    {
        public string UserName { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string GameName { get; set; }
        public float AmountStaked { get; set; }
        public float AmountWon { get; set; }
        public DateTime PlayedAt { get; set; }

    }
}
