using ReimbursementTrackingApplication.Models.DTOs;

namespace ReimbursementTrackingApplication.Interfaces
{
    public interface IApprovalService
    {
       
            Task<SuccessResponseDTO<ResponseApprovalStageDTO>> GetApprovalByIdAsync(int approvalId);
            Task<PaginatedResultDTO<ResponseApprovalStageDTO>> GetApprovalsByRequestIdAsync(int requestId, int pageNumber, int pageSize);
            Task<PaginatedResultDTO<ResponseApprovalStageDTO>> GetApprovalsByReviewerIdAsync(int reviewerId, int pageNumber, int pageSize);

            
            Task<PaginatedResultDTO<ResponseApprovalStageDTO>> GetHrPendingApprovalsAsync(int hrReviewerId, int pageNumber, int pageSize);

           
            Task<PaginatedResultDTO<ResponseApprovalStageDTO>> GetFinancePendingApprovalsAsync(int financeReviewerId, int pageNumber, int pageSize);

            Task<SuccessResponseDTO<int>> ApproveRequestAsync(ApprovalStageDTO approval);
            Task<SuccessResponseDTO<int>> RejectRequestAsync(ApprovalStageDTO approval);
        

    }
}
