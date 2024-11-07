using AutoMapper;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Models.DTOs;

namespace ReimbursementTrackingApplication.Services
{
    public class ApprovalService : IApprovalService
    {

        private readonly IRepository<int, ApprovalStage> _repository;
        private readonly IRepository<int, User> _userRepository;
        private readonly IRepository<int,ReimbursementRequest> _requestRepository;
        private readonly IMapper _mapper;

        public ApprovalService(IRepository<int,ApprovalStage> repository, IRepository<int, User> userRepository, IRepository<int, ReimbursementRequest> requestRepository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
            _userRepository = userRepository;
            _requestRepository=requestRepository;
        }
        public async Task<SuccessResponseDTO<int>> ApproveRequestAsync(ApprovalStageDTO approval)
        {
            try
            {
                var reviewer = await _userRepository.Get(approval.ReviewId);
                
                Departments department = reviewer.Department;
                var request = await _requestRepository.Get(approval.RequestId);
                ApprovalStage approvalStage = _mapper.Map<ApprovalStage>(approval);
                if(department==Departments.HR)
                {
                    approvalStage.Stage = Stage.Hr;
         
                    request.Stage = Stage.Hr;

                }else if(department==Departments.Finance)
                {
                    approvalStage.Stage = Stage.Financial;
                    request.Stage = Stage.Financial;
                    request.Status = RequestStatus.Passed;
                }
                var addApproval = await _repository.Add(approvalStage);
                return new SuccessResponseDTO<int>
                {
                    IsSuccess = true,
                    Message = "Approved Added",
                    Data = addApproval.Id,
                };

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<SuccessResponseDTO<ResponseApprovalStageDTO>> GetApprovalByIdAsync(int approvalId)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedResultDTO<ResponseApprovalStageDTO>> GetApprovalsByRequestIdAsync(int requestId, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedResultDTO<ResponseApprovalStageDTO>> GetApprovalsByReviewerIdAsync(int reviewerId, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedResultDTO<ResponseApprovalStageDTO>> GetFinancePendingApprovalsAsync(int financeReviewerId, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedResultDTO<ResponseApprovalStageDTO>> GetHrPendingApprovalsAsync(int hrReviewerId, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<SuccessResponseDTO<int>> RejectRequestAsync(ApprovalStageDTO approval)
        {
            throw new NotImplementedException();
        }
    }
}
