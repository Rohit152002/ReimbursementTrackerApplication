using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Models.DTOs;

namespace ReimbursementTrackingApplication.Interfaces
{
    public interface IReimbursementRequestService
    {
        Task<SuccessResponseDTO<ResponseReimbursementRequestDTO>> GetRequestByIdAsync(int requestId);
        Task<PaginatedResultDTO<ResponseReimbursementRequestDTO>> GetRequestsByUserIdAsync(int userId, int pageNumber, int pageSize);

        Task<PaginatedResultDTO<ResponseReimbursementRequestDTO>> GetRequestsByManagerIdAsync(int managerId, int pageNumber, int pageSize);
        Task<PaginatedResultDTO<ResponseReimbursementRequestDTO>> GetRequestsByStatusAsync(RequestStatus status, int pageNumber, int pageSize);
        Task<PaginatedResultDTO<ResponseReimbursementRequestDTO>> GetRequestsByPolicyAsync(int policyId, int pageNumber, int pageSize);
        Task<SuccessResponseDTO<ResponseReimbursementRequestDTO>> SubmitRequestAsync(CreateReimbursementRequestDTO requestDto);
        Task<PaginatedResultDTO<ResponseReimbursementRequestDTO>> GetAllRequest( int pageNumber, int pageSize);

        Task<SuccessResponseDTO<int>> DeleteRequestAsync(int requestId);
    }
}
