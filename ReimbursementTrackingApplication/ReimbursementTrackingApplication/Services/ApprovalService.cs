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
        private readonly IRepository<int, ReimbursementRequest> _requestRepository;
        private readonly IRepository<int, Policy> _policyRepository;
        private readonly IRepository<int, Employee> _employeeRepository;
        //private readonly IReimbursementItemService _itemService;
        private readonly IRepository<int, Payment> _paymentRepository;
        private readonly IRepository<int, ReimbursementItem> _itemRepository;
        private readonly IRepository<int, ExpenseCategory> _expenseCategoryRepository;
        private readonly IMapper _mapper;
        private readonly IMailSender _mailSender;

        public ApprovalService(
            IRepository<int, ApprovalStage> repository, IRepository<int, User> userRepository,
            IRepository<int, ReimbursementRequest> requestRepository,
            IRepository<int, Employee> employeeRepository,
            //IReimbursementItemService itemService,
            IRepository<int, Policy> policyRepository,
            IRepository<int, Payment> paymentRepository,
            IRepository<int, ReimbursementItem> itemRepository,
IRepository<int, ExpenseCategory> expenseCategoryRepository, IMapper mapper, IMailSender mailSender)
        {
            _repository = repository;
            _mapper = mapper;
            _userRepository = userRepository;
            _requestRepository = requestRepository;
            _policyRepository = policyRepository;
            _paymentRepository = paymentRepository;
            //_itemService = itemService;
            _itemRepository = itemRepository;
            _expenseCategoryRepository = expenseCategoryRepository;
            _mailSender = mailSender;
            _employeeRepository = employeeRepository;
        }
        public async Task<SuccessResponseDTO<int>> ApproveRequestAsync(ApprovalStageDTO approval)
        {
            try
            {
                var reviewer = await _userRepository.Get(approval.ReviewId);

                if (reviewer.Department == null && reviewer == null)
                {
                    throw new Exception($"{approval.ReviewId}");
                }
                var approvals = await _repository.GetAll();
                var existApproval = approvals.FirstOrDefault(r => r.RequestId == approval.RequestId && r.ReviewId == approval.ReviewId);
                if (existApproval != null && existApproval.Status == Status.Approved)
                {
                    throw new Exception("Already Approved");
                }

                Departments department = (Departments)reviewer.Department;
                var request = await _requestRepository.Get(approval.RequestId);
                string passedBy;

                bool isAuthorizedDepartment = department == Departments.HR || department == Departments.Finance;

                var employees = (await _employeeRepository.GetAll()).FirstOrDefault(r => r.EmployeeId == request.UserId && r.ManagerId == reviewer.Id);

                if (!isAuthorizedDepartment && employees == null)
                {
                    throw new UnauthorizedAccessException("Unauthorize");
                }

                ApprovalStage approvalStage = _mapper.Map<ApprovalStage>(approval);

                if (department == Departments.HR && request.Stage == Stage.Manager)
                {
                    approvalStage.Stage = Stage.Hr;

                    request.Stage = Stage.Hr;
                    passedBy = "HR";

                }
                else if (department == Departments.Finance && request.Stage == Stage.Hr)
                {
                    approvalStage.Stage = Stage.Financial;
                    passedBy = "Finance";
                    request.Stage = Stage.Financial;
                    request.Status = RequestStatus.Passed;
                    Payment payment = new Payment()
                    {
                        RequestId = request.Id,
                        AmountPaid = request.TotalAmount,
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

                var paymentPendingMessage = request.Stage == Stage.Financial ? "<p><strong>Payment is pending:</strong> Your reimbursement request has been approved by Finance. Please wait for the payment to be processed.</p>" : "";

                var StagePassed = request.Stage == Stage.Manager ? "HR" : request.Stage == Stage.Hr ? "Finance" : "Payment";

                var htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
            color: #333333;
        }}
        .email-container {{
            background-color: #ffffff;
            width: 100%;
            max-width: 600px;
            margin: 20px auto;
            border: 1px solid #dddddd;
            border-radius: 8px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            overflow: hidden;
        }}
        .email-header {{
            background-color: #28a745;
            padding: 20px;
            color: white;
            text-align: center;
        }}
        .email-header h1 {{
            margin: 0;
            font-size: 24px;
        }}
        .email-body {{
            padding: 20px;
            line-height: 1.6;
        }}
        .email-body p {{
            font-size: 16px;
            margin-bottom: 10px;
        }}
        .email-footer {{
            padding: 10px;
            background-color: #f4f4f4;
            text-align: center;
            font-size: 14px;
            color: #888888;
        }}
        .email-footer a {{
            color: #28a745;
            text-decoration: none;
        }}
        .button {{
            background-color: #007bff;
            color: white;
            padding: 10px 20px;
            border-radius: 5px;
            text-decoration: none;
            display: inline-block;
            margin-top: 20px;
        }}
        .button:hover {{
            background-color: #0056b3;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='email-header'>
            <h1>Request Approval Notification</h1>
        </div>
        <div class='email-body'>
            <p>Dear {user.UserName},</p>
            <p>We are pleased to inform you that your reimbursement request has been <strong>approved</strong> by the {passedBy} department.</p>
            <p>The approval was made by {reviewer.UserName}, and your request has now advanced to the {StagePassed} stage.</p>
            {paymentPendingMessage}
            <p>If you have any questions regarding the approval process or your request, feel free to contact us.</p>
            <p>To view the details of your request or the current status, please log in to the Reimbursement Tracker</p>

        </div>
        <div class='email-footer'>
            <p>Thank you for using Reimbursement Tracker.</p>
            <p><a href='#'>Visit our website</a> | <a href='mailto:support@reimbursementtracker.com'>Contact Support</a></p>
        </div>
    </div>
</body>
</html>
";

                var message = new Message(new string[] { user.Email }, "Request Approved", htmlContent);
                _mailSender.SendEmail(message);

                return new SuccessResponseDTO<int>
                {
                    IsSuccess = true,
                    Message = "Approved Added",
                    Data = addApproval.Id,
                };

            }
            catch (UnauthorizedAccessException ex)
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
                if (reviewer.Department == null && reviewer == null)
                {
                    throw new Exception($"{approval.ReviewId}");
                }

                Departments department = (Departments)reviewer.Department;

                var request = await _requestRepository.Get(approval.RequestId);
                bool isAuthorizedDepartment = department == Departments.HR || department == Departments.Finance;

                var user = await _userRepository.Get(request.UserId);


                var employees = (await _employeeRepository.GetAll()).FirstOrDefault(r => r.EmployeeId == request.UserId && r.ManagerId == reviewer.Id);

                if (!isAuthorizedDepartment && employees == null)
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
                var htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
            color: #333333;
        }}
        .email-container {{
            background-color: #ffffff;
            width: 100%;
            max-width: 600px;
            margin: 20px auto;
            border: 1px solid #dddddd;
            border-radius: 8px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            overflow: hidden;
        }}
        .email-header {{
            background-color: #dc3545;
            padding: 20px;
            color: white;
            text-align: center;
        }}
        .email-header h1 {{
            margin: 0;
            font-size: 24px;
        }}
        .email-body {{
            padding: 20px;
            line-height: 1.6;
        }}
        .email-body p {{
            font-size: 16px;
            margin-bottom: 10px;
        }}
        .email-footer {{
            padding: 10px;
            background-color: #f4f4f4;
            text-align: center;
            font-size: 14px;
            color: #888888;
        }}
        .email-footer a {{
            color: #dc3545;
            text-decoration: none;
        }}
        .button {{
            background-color: #dc3545;
            color: white;
            padding: 10px 20px;
            border-radius: 5px;
            text-decoration: none;
            display: inline-block;
            margin-top: 20px;
        }}
        .button:hover {{
            background-color: #bd2130;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='email-header'>
            <h1>Request Rejected</h1>
        </div>
        <div class='email-body'>
            <p>Dear {user.UserName},</p>
            <p>We regret to inform you that your reimbursement request has been <strong>rejected</strong> by the {reviewer.Department} department.</p>
            <p>The rejection was processed by {reviewer.UserName}, and your request has been marked as <strong>Rejected</strong> in the system.</p>
            <p>If you have any questions regarding this decision or need further clarification, feel free to reach out to our support team for more information.</p>
            <p>To view the details of your request or the current status, please log in to the Reimbursement Tracker:</p>
            <a href='#' class='button'>View Your Request</a>
        </div>
        <div class='email-footer'>
            <p>Thank you for using Reimbursement Tracker.</p>
            <p><a href='#'>Visit our website</a> | <a href='mailto:support@reimbursementtracker.com'>Contact Support</a></p>
        </div>
    </div>
</body>
</html>
";
                var message = new Message(new string[] { user.Email }, "Request Rejected", htmlContent);
                _mailSender.SendEmail(message);
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
                var requestUser = await _userRepository.Get(request.UserId);
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
                    Id = approval.Id,
                    RequestId = request.Id,
                    Request = requestDTO,
                    ReviewId = reviewUser.Id,
                    Review = reviewUserDTO,
                    Comments = approval.Comments,
                    RequestStage = approval.Status,
                };

                return new SuccessResponseDTO<ResponseApprovalStageDTO>()
                {
                    IsSuccess = true,
                    Message = "Fetch Successfully",
                    Data = responseApproval
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PaginatedResultDTO<ResponseApprovalStageDTO>> GetApprovalsByRequestIdAsync(int requestId, int pageNumber, int pageSize)
        {
            try
            {
                var approvals = (await _repository.GetAll()).Where(a => a.IsDeleted == false && a.RequestId == requestId).ToList();

                return await GetResponses(approvals, pageNumber, pageSize);


            }
            catch (Exception ex)
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
                var approvals = (await _repository.GetAll()).Where(a => a.IsDeleted == false && a.Stage == Stage.Hr && a.Status == Status.Approved).ToList();

                return await GetResponses(approvals, pageNumber, pageSize);


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PaginatedResultDTO<ResponseApprovalStageDTO>> GetHrPendingApprovalsAsync(int pageNumber, int pageSize)
        {
            try
            {
                var approvals = (await _repository.GetAll()).Where(a => a.IsDeleted == false && a.Stage == Stage.Manager && a.Status == Status.Approved).ToList();

                // var hrApprovals = (await _repository.GetAll()).Where(a => a.IsDeleted == false && a.Stage == Stage.Hr && a.Status == Status.Approved).ToList();

                // approvals = approvals.Where(a => !hrApprovals.Any(hr => hr.RequestId == a.RequestId)).ToList();

                return await GetResponses(approvals, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PaginatedResultDTO<ResponseApprovalStageDTO>> GetAllApprovalsAsync(int pageNumber, int pageSize)
        {
            try
            {
                var approvals = (await _repository.GetAll()).Where(a => a.IsDeleted == false).ToList();

                return await GetResponses(approvals, pageNumber, pageSize);


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<PaginatedResultDTO<ResponseApprovalStageDTO>> GetResponses(List<ApprovalStage> approvals, int pageNumber, int pageSize)
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
                Console.WriteLine(request.TotalAmount);
                ResponseReimbursementRequestDTO requestDTO = new ResponseReimbursementRequestDTO()
                {
                    Id = request.Id,
                    UserId = request.UserId,
                    Comments = request.Comments,
                    Stage = request.Stage,
                    Status = request.Status,
                    User = requestUserDTO,
                    TotalAmount = request.TotalAmount,
                    PolicyId = request.PolicyId,
                    PolicyName = (await _policyRepository.Get(request.PolicyId)).PolicyName,
                    Items = itemsDTO,
                    IsApprovedByManager = request.Stage > Stage.Processing ? true : false,
                    IsApprovedByHr = request.Stage > Stage.Manager ? true : false,
                    IsApprovedByFinance = request.Stage > Stage.Hr ? true : false,

                };

                ResponseApprovalStageDTO responseApproval = new ResponseApprovalStageDTO()
                {
                    Id = approval.Id,
                    RequestId = request.Id,
                    Request = requestDTO,
                    ReviewId = reviewUser.Id,
                    Review = reviewUserDTO,
                    Comments = approval.Comments,
                    ReviewDate = approval.ReviewDate,
                    RequestStage = approval.Status,
                    Stage = approval.Stage


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
