namespace ReimbursementTrackingApplication.Models.DTOs
{
    public class PaginatedResultDTO<T>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public List<T> Data { get; set; }

           
    }
}
