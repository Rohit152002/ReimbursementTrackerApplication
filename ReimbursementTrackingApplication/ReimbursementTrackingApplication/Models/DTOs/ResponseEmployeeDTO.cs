namespace ReimbursementTrackingApplication.Models.DTOs
{
    public class ResponseEmployeeDTO
    {
        public int EmployeeId { get; set; }
        public UserDTO Employee { get; set; }
        public int ManagerId { get; set; }
        public UserDTO Manager { get; set; }
    }
}
