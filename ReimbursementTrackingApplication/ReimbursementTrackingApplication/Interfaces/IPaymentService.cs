using ReimbursementTrackingApplication.Models.DTOs;

namespace ReimbursementTrackingApplication.Interfaces
{
    public interface IPaymentService
    {
        Task<SuccessResponseDTO<ResponsePayment>> GetPaymentByRequestIdAsync(int requestId);
        Task<PaginatedResultDTO<ResponsePayment>> GetPaymentsByUserIdAsync(int userId, int pageNumber, int pageSize);
        Task<PaginatedResultDTO<ResponsePayment>> GetAllPaymentsAsync(int pageNumber, int pageSize);
        Task<SuccessResponseDTO<ResponsePayment>> ProcessPaymentAsync(PaymentDTO payment);
     
        Task<SuccessResponseDTO<int>> DeletePaymentAsync(int paymentId);
    }
}
