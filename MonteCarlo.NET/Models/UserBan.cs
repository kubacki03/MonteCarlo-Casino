namespace MonteCarlo.NET.Models
{
	public class UserBan
	{
        public string UserName { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime Date { get; set; }
        public int DurationDays { get; set; }
        public string Reason { get; set; }
    }
}
