using ReimbursementTrackingApplication.Models.DTOs;

namespace ReimbursementTrackingApplication.Interfaces
{
    public interface IPolicyService
    {
        Task<SuccessResponseDTO<ResponsePolicyDTO>> GetPolicyByIdAsync(int policyId);
        Task<PaginatedResultDTO<ResponsePolicyDTO>> GetAllPolicesAsync(int pageNumber, int pageSize);
        Task<SuccessResponseDTO<int>> AddPolicyAsync(CreatePolicyDTO policyDTO);
        Task<SuccessResponseDTO<int>> UpdatePolicyAsync(int categoryId, CreatePolicyDTO policyDTO);
        Task<SuccessResponseDTO<int>> DeletePolicyAsync(int policyId);
    }
}
