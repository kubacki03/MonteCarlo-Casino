namespace MonteCarlo.NET.Models
{
	public class UserTransaction
	{
        public string UserName { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime Date { get; set; }
        public float Amount { get; set; }
        public string Type { get; set; }
    }
}
