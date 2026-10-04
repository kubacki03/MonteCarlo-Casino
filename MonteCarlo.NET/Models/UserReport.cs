namespace MonteCarlo.NET.Models
{
	public class UserReport
	{
        public int ReportId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
    }
}
