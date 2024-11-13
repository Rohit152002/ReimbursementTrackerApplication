namespace ReimbursementTrackingApplication.Models
{
    public class EmailConfiguration
    {
        public string From { get; set; } = "laishramrohit15@gmail.com";
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
