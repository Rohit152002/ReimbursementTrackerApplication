namespace ReimbursementTrackingApplication.Models.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Departments Department { get; set; }
    }
}
