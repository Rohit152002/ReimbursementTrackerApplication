namespace ReimbursementTrackingApplication.Models
{
    public class EmailConfiguration
    {
        public string From { get; set; } = "laishramrohit15@gmail.com";
        public string SmtpServer { get; set; } = string.Empty;
        public int Port { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
