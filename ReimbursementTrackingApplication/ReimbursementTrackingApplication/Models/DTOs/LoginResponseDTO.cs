namespace ReimbursementTrackingApplication.Models.DTOs
{
    public class LoginResponseDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Departments Department { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
