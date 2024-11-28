namespace ReimbursementTrackingApplication.Models.DTOs
{
    public class UserTokenDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string IsManager { get; set; } = string.Empty;
    }
}
