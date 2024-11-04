namespace ReimbursementTrackingApplication.Models.DTOs
{
    public class ChangePasswordDTO
    {
        public int UserId;
        public string currentPassword { get; set; } =string.Empty;
        public string newPassword { get; set; }= string.Empty;
        public string confirmPassword { get; set; }=string.Empty;   
    }
}
