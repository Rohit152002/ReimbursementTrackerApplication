using AutoMapper;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Models.DTOs;
using ReimbursementTrackingApplication.Repositories;

namespace ReimbursementTrackingApplication.Services
{
    public class ApprovalService : IApprovalService
    {

        private readonly IRepository<int, ApprovalStage> _repository;
        private readonly IRepository<int, User> _userRepository;
        private readonly IRepository<int,ReimbursementRequest> _requestRepository;
        private readonly IRepository<int,Policy> _policyRepository;
        private readonly IRepository<int, Employee> _employeeRepository;
        //private readonly IReimbursementItemService _itemService;
        private readonly IRepository<int ,Payment> _paymentRepository;
        private readonly IRepository<int, ReimbursementItem> _itemRepository;
        private readonly IRepository<int,ExpenseCategory> _expenseCategoryRepository;
        private readonly IMapper _mapper;
        private readonly IMailSender _mailSender;

        public ApprovalService(
            IRepository<int,ApprovalStage> repository, IRepository<int, User> userRepository,
            IRepository<int, ReimbursementRequest> requestRepository,
            IRepository<int,Employee> employeeRepository,
            //IReimbursementItemService itemService,
            IRepository<int,Policy> policyRepository,
            IRepository<int,Payment> paymentRepository,
            IRepository<int,ReimbursementItem> itemRepository,
IRepository<int,ExpenseCategory> expenseCategoryRepository, IMapper mapper,IMailSender mailSender)
        {
            _repository = repository;
            _mapper = mapper;
            _userRepository = userRepository;
            _requestRepository=requestRepository;
            _policyRepository = policyRepository;
            _paymentRepository = paymentRepository;
            //_itemService = itemService;
            _itemRepository= itemRepository;
            _expenseCategoryRepository= expenseCategoryRepository;
            _mailSender = mailSender;
            _employeeRepository = employeeRepository;
        }
        public async Task<SuccessResponseDTO<int>> ApproveRequestAsync(ApprovalStageDTO approval)
        {
            try
            {
                var reviewer = await _userRepository.Get(approval.ReviewId);

                Departments department = reviewer.Department;
                var request = await _requestRepository.Get(approval.RequestId);
                string passedBy;

                bool isAuthorizedDepartment = department== Departments.HR || department==Departments.Finance;

                var employees = (await _employeeRepository.GetAll()).FirstOrDefault(r=>r.EmployeeId==request.UserId && r.ManagerId==reviewer.Id);

                if(!isAuthorizedDepartment && employees==null)
                {
                    throw new UnauthorizedAccessException("Unauthorize");
                }

                ApprovalStage approvalStage = _mapper.Map<ApprovalStage>(approval);

                if(department==Departments.HR && request.Stage==Stage.Manager)
                {
                    approvalStage.Stage = Stage.Hr;

                    request.Stage = Stage.Hr;
                    passedBy = "HR";

                }else if(department==Departments.Finance && request.Stage==Stage.Hr)
                {
                    approvalStage.Stage = Stage.Financial;
                    passedBy = "Finance";
                    request.Stage = Stage.Financial;
                    request.Status = RequestStatus.Passed;
                    Payment payment = new Payment()
                    {
                        RequestId=request.Id,
                        AmountPaid=request.TotalAmount,
                    };
                    await _paymentRepository.Add(payment);

                }
                else
                {
                    approvalStage.Stage = Stage.Manager;
                    request.Stage = Stage.Manager;
                    passedBy = "Manager";
                }
                await _requestRepository.Update(approval.RequestId, request);
                var addApproval = await _repository.Add(approvalStage);

                var user = await _userRepository.Get(request.UserId);


                var message = new Message(new string[] { user.Email }, "Request Approved", $"Request approved by {passedBy} {reviewer.UserName}  .");
                _mailSender.SendEmail(message);

                return new SuccessResponseDTO<int>
                {
                    IsSuccess = true,
                    Message = "Approved Added",
                    Data = addApproval.Id,
                };

            }
            catch(UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<SuccessResponseDTO<int>> RejectRequestAsync(ApprovalStageDTO approval)
        {
            try
            {
                var reviewer = await _userRepository.Get(approval.ReviewId);

                Departments department = reviewer.Department;
                var request = await _requestRepository.Get(approval.RequestId);
                bool isAuthorizedDepartment = department== Departments.HR || department==Departments.Finance;

                var employees = (await _employeeRepository.GetAll()).FirstOrDefault(r=>r.EmployeeId==request.UserId && r.ManagerId==reviewer.Id);

                if(!isAuthorizedDepartment && employees==null)
                {
                    throw new UnauthorizedAccessException("Unauthorize");
                }


                ApprovalStage approvalStage = _mapper.Map<ApprovalStage>(approval);
                if (department == Departments.HR)
                {
                    approvalStage.Stage = Stage.Hr;
                    request.Stage = Stage.Hr;
                    approvalStage.Status = Status.Rejected;
                    request.Status = RequestStatus.Rejected;

                }
                else if (department == Departments.Finance)
                {
                    approvalStage.Stage = Stage.Financial;
                    request.Stage = Stage.Financial;
                    approvalStage.Status = Status.Rejected;
                    request.Status = RequestStatus.Rejected;
                }
                else
                {
                    approvalStage.Stage = Stage.Manager;
                    request.Stage = Stage.Manager;
                    approvalStage.Status = Status.Rejected;
                    request.Status = RequestStatus.Rejected;
                }
                await _requestRepository.Update(approval.RequestId, request);
                var addApproval = await _repository.Add(approvalStage);
                return new SuccessResponseDTO<int>
                {
                    IsSuccess = true,
                    Message = "Rejected Added",
                    Data = addApproval.Id,
                };

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<SuccessResponseDTO<ResponseApprovalStageDTO>> GetApprovalByIdAsync(int approvalId)
        {
            try
            {

            var approval = await _repository.Get(approvalId);
            var reviewUser = await _userRepository.Get(approval.ReviewId);
            var request = await _requestRepository.Get(approval.RequestId);
            var requestUser= await _userRepository.Get(request.UserId);
            var items = (await _itemRepository.GetAll()).Where(r => r.RequestId == approval.RequestId && r.IsDeleted == false).ToList();
            var total = items.Count();

            var itemsDTO = _mapper.Map<List<ResponseReimbursementItemDTO>>(items);
                foreach (var item in itemsDTO)
                {
                    item.CategoryName = (await _expenseCategoryRepository.Get(item.CategoryId)).Name;
                }


            UserDTO reviewUserDTO = _mapper.Map<UserDTO>(reviewUser);
            UserDTO requestUserDTO = _mapper.Map<UserDTO>(requestUser);


            ResponseReimbursementRequestDTO requestDTO = new ResponseReimbursementRequestDTO()
            {
                Id = request.Id,
                UserId = request.UserId,
                User = requestUserDTO,
                PolicyId = request.PolicyId,
                PolicyName = (await _policyRepository.Get(request.PolicyId)).PolicyName,
                Items = itemsDTO

            };

            ResponseApprovalStageDTO responseApproval = new ResponseApprovalStageDTO()
            {
                Id= approval.Id,
                RequestId = request.Id,
                Request=requestDTO,
                ReviewId=reviewUser.Id,
                Review=reviewUserDTO,
                Comments = approval.Comments,
            };

            return new SuccessResponseDTO<ResponseApprovalStageDTO>()
            {
                IsSuccess = true,
                Message = "Fetch Successfully",
                Data = responseApproval
            };
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PaginatedResultDTO<ResponseApprovalStageDTO>> GetApprovalsByRequestIdAsync(int requestId, int pageNumber, int pageSize)
        {
            try
            {
                var approvals = (await _repository.GetAll()).Where(a=>a.IsDeleted==false && a.RequestId==requestId).ToList();

                return await GetResponses(approvals, pageNumber, pageSize);


            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PaginatedResultDTO<ResponseApprovalStageDTO>> GetApprovalsByReviewerIdAsync(int reviewerId, int pageNumber, int pageSize)
        {
            try
            {
                var approvals = (await _repository.GetAll()).Where(a => a.IsDeleted == false && a.ReviewId == reviewerId).ToList();

                return await GetResponses(approvals, pageNumber, pageSize);


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PaginatedResultDTO<ResponseApprovalStageDTO>> GetFinancePendingApprovalsAsync(int pageNumber, int pageSize)
        {
            try
            {
                var approvals = (await _repository.GetAll()).Where(a => a.IsDeleted == false && a.Stage == Stage.Hr && a.Status==Status.Approved).ToList();

                return await GetResponses(approvals, pageNumber, pageSize);


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PaginatedResultDTO<ResponseApprovalStageDTO>> GetHrPendingApprovalsAsync( int pageNumber, int pageSize)
        {
            try
            {
                var approvals = (await _repository.GetAll()).Where(a => a.IsDeleted == false && a.Stage == Stage.Manager && a.Status == Status.Approved).ToList();

                return await GetResponses(approvals, pageNumber, pageSize);


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<PaginatedResultDTO<ResponseApprovalStageDTO>> GetResponses(List<ApprovalStage> approvals, int pageNumber,int pageSize)
        {
            List<ResponseApprovalStageDTO> responseApprovalStageDTOs = new List<ResponseApprovalStageDTO>();


            foreach (var approval in approvals)
            {
                var reviewUser = await _userRepository.Get(approval.ReviewId);
                var request = await _requestRepository.Get(approval.RequestId);
                var requestUser = await _userRepository.Get(request.UserId);
                var items = (await _itemRepository.GetAll()).Where(r => r.RequestId == approval.RequestId && r.IsDeleted == false).ToList();

                var itemsDTO = _mapper.Map<List<ResponseReimbursementItemDTO>>(items);
                foreach (var item in itemsDTO)
                {
                    item.CategoryName = (await _expenseCategoryRepository.Get(item.CategoryId)).Name;
                }

                UserDTO reviewUserDTO = _mapper.Map<UserDTO>(reviewUser);
                UserDTO requestUserDTO = _mapper.Map<UserDTO>(requestUser);
                ResponseReimbursementRequestDTO requestDTO = new ResponseReimbursementRequestDTO()
                {
                    Id = request.Id,
                    UserId = request.UserId,
                    User = requestUserDTO,
                    PolicyId = request.PolicyId,
                    PolicyName = (await _policyRepository.Get(request.PolicyId)).PolicyName,
                    Items = itemsDTO

                };

                ResponseApprovalStageDTO responseApproval = new ResponseApprovalStageDTO()
                {
                    Id = approval.Id,
                    RequestId = request.Id,
                    Request = requestDTO,
                    ReviewId = reviewUser.Id,
                    Review = reviewUserDTO,
                    Comments = approval.Comments,
                };

                responseApprovalStageDTOs.Add(responseApproval);
            }
            var total = responseApprovalStageDTOs.Count();
            var pagedItems = responseApprovalStageDTOs
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedResultDTO<ResponseApprovalStageDTO>
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                Data = pagedItems

            };
        }

    }
}
