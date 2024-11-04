using System.ComponentModel.DataAnnotations;

namespace ReimbursementTrackingApplication.Models.DTOs
{
    public class UserCreateDTO
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long and cannot exceed 100 characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        public Departments Department { get; set; }
    }
}
