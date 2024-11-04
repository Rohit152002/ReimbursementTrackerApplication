namespace ReimbursementTrackingApplication.Models.DTOs
{
    public class UserCreateDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; }
        public Departments Department { get; set; }
    }
}
