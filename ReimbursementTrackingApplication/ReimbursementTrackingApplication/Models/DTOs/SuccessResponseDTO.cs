namespace ReimbursementTrackingApplication.Models.DTOs
{
    public class SuccessResponseDTO<T>
    {
        public bool IsSuccess { get; set; }   
        public string Message { get; set; }   
        public T Data { get; set; }          
        public DateTime Timestamp { get; set; }   

        public SuccessResponseDTO()
        {
            Timestamp = DateTime.UtcNow;
        }
    }
}
