using AutoMapper;
using ReimbursementTrackingApplication.Exceptions;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Models.DTOs;
using ReimbursementTrackingApplication.Repositories;
using System.Reflection.Metadata.Ecma335;

namespace ReimbursementTrackingApplication.Services
{
    public class ReimbursementRequestService : IReimbursementRequestService
    {
        private readonly IRepository<int, ReimbursementRequest> _repository;
        private readonly IRepository<int, Policy> _policyRepository;

        private readonly string _uploadFolder;
        private readonly IRepository<int, ReimbursementItem> _itemRepository;

        private readonly IRepository<int, Employee> _employeeRepository;
        private readonly IRepository<int, ExpenseCategory> _categoryRepository;
        private readonly IRepository<int, User> _userRepository;

        private readonly IMapper _mapper;
        private readonly IRepository<int, BankAccount> _bankAccountRepository;
        private readonly IRepository<int, ApprovalStage> _approvalStageRepository;
        public ReimbursementRequestService(
            IRepository<int, ReimbursementRequest> repository,
            IRepository<int, Policy> policyRepository,
            IMapper mapper,
             string uploadFolder,
        IRepository<int, ReimbursementItem> itemRepository,
            IRepository<int, Employee> employeeRepository,
            IRepository<int, ExpenseCategory> categoryRepository,
            IRepository<int, User> userRepository,
            IRepository<int, BankAccount> bankAccRepository,
            IRepository<int, ApprovalStage> approvalStageRepository)
        {
            _userRepository = userRepository;
            _repository = repository;
            _policyRepository = policyRepository;

            _mapper = mapper;
            _uploadFolder = uploadFolder;

            _itemRepository = itemRepository;
            _employeeRepository = employeeRepository;
            _categoryRepository = categoryRepository;
            _bankAccountRepository = bankAccRepository;
            _approvalStageRepository = approvalStageRepository;
        }

        public async Task<SuccessResponseDTO<ResponseReimbursementRequestDTO>> SubmitRequestAsync(CreateReimbursementRequestDTO requestDto)
        {
            try
            {
                var request = MapRequest(requestDto);
                var currentYear = DateTime.Now.Year;
                var existingRequest = (await _repository.GetAll()).Where(r => r.UserId == requestDto.UserId && r.PolicyId == requestDto.PolicyId && r.CreatedAt.Year == currentYear &&( r.Status == RequestStatus.Passed || r.Status==RequestStatus.Pending)).ToList();
                var employee = (await _employeeRepository.GetAll()).FirstOrDefault(e => e.EmployeeId == requestDto.UserId);
                if (employee == null)
                {
                    throw new UnauthorizedException("Assign a Manager First");
                }

                if (existingRequest.Any())
                {
                    throw new Exception("You have already submitted a request for this policy this year.");
                }

                var banks = await _bankAccountRepository.GetAll();
                var bank = banks.FirstOrDefault(b => b.UserId == requestDto.UserId) ?? throw new NotFoundException("Bank");
                var requestAdded = await _repository.Add(request);

                var items = (await AddItemsAsync(requestDto.Items, requestAdded.Id)).ToList();


                request.Items = _mapper.Map<List<ReimbursementItem>>(items);


                var responseRequest = await CreateResponseDTO(requestAdded, items, requestDto.UserId, requestDto.PolicyId);


                return new SuccessResponseDTO<ResponseReimbursementRequestDTO>
                {
                    IsSuccess = true,
                    Message = "Request Created Successfully",
                    Data = responseRequest
                };

            }
            catch (UnauthorizedException ex)
            {
                throw new UnauthorizedException(ex.Message);
            }
            catch (CollectionEmptyException ex)
            {
                throw new UnauthorizedException(ex.Message);
            }
            catch (NotFoundException)
            {
                throw new Exception("Bank details not found. Please add your bank information to submit the reimbursement request.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        private async Task<List<ResponseReimbursementItemDTO>> AddItemsAsync(IEnumerable<RequestITemDTO> itemDTOs, int id)
        {
            List<ResponseReimbursementItemDTO> responseItemDTOs = new List<ResponseReimbursementItemDTO>();

            List<ReimbursementItemDTO> items = new List<ReimbursementItemDTO>();
            foreach (var item in itemDTOs)
            {
                var new_item = new ReimbursementItemDTO()
                {
                    RequestId = id,
                    Amount = item.Amount,
                    CategoryId = item.CategoryId,
                    Description = item.Description,
                    receiptFile = item.receiptFile
                };
                items.Add(new_item);
                var result = await AddItemService(new_item);
                responseItemDTOs.Add(result);
            }

            return responseItemDTOs;
        }

        private ReimbursementRequest MapRequest(CreateReimbursementRequestDTO requestDto)
        {
            double totalsum = 0;
            foreach (var item in requestDto.Items)
            {
                totalsum += item.Amount;
            }
            return new ReimbursementRequest
            {
                UserId = requestDto.UserId,
                PolicyId = requestDto.PolicyId,
                TotalAmount = totalsum,
                Comments = requestDto.Comments,
            };

        }

        private async Task<ResponseReimbursementRequestDTO> CreateResponseDTO(ReimbursementRequest requestAdded, List<ResponseReimbursementItemDTO> items, int userId, int policyId)
        {
            var responseRequest = _mapper.Map<ResponseReimbursementRequestDTO>(requestAdded);
            responseRequest.Items = items;
            responseRequest.User = await GetUser(userId);
            responseRequest.PolicyName = (await _policyRepository.Get(policyId)).PolicyName;
            return responseRequest;
        }
        public async Task<UserDTO> GetUser(int id)
        {
            var user = await _userRepository.Get(id);
            var userDTO = _mapper.Map<UserDTO>(user);
            return userDTO;

        }

        public async Task<SuccessResponseDTO<ResponseReimbursementRequestDTO>> GetRequestByIdAsync(int requestId)
        {
            try
            {
                ReimbursementRequest request = await _repository.Get(requestId);

                ResponseReimbursementRequestDTO requestDTO = _mapper.Map<ResponseReimbursementRequestDTO>(request);

                requestDTO.PolicyName = (await _policyRepository.Get(request.PolicyId)).PolicyName;
                requestDTO.Items = await GetItemsByRequestId(requestDTO.Id);
                requestDTO.User = await GetUser(request.UserId);
                requestDTO.DateTime = request.CreatedAt;
                return new SuccessResponseDTO<ResponseReimbursementRequestDTO>()
                {
                    IsSuccess = true,
                    Message = "Data Fetch Successfully",
                    Data = requestDTO

                };

            }
            catch (Exception ex)
            {
                throw new NotFoundException(ex.Message);
            }

        }

        public async Task<PaginatedResultDTO<ResponseReimbursementRequestDTO>> GetRequestsByUserIdAsync(int userId, int pageNumber, int pageSize)
        {
            try
            {
                var requests = await _repository.GetAll();
                var filter_request = requests.Where(r => r.UserId == userId && r.IsDeleted == false).ToList();

                var total = filter_request.Count();
                var pagedItems = filter_request
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();


                List<ResponseReimbursementRequestDTO> requestDTOs = await MappingReimbursementRequest(filter_request);

                // foreach(var request in requestDTOs)
                // {
                //     request.Items = await GetItemsByRequestId(request.Id);

                // }

                return new PaginatedResultDTO<ResponseReimbursementRequestDTO>
                {
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalCount = total,
                    TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                    Data = requestDTOs

                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ResponseReimbursementItemDTO>> GetItemsByRequestId(int requestId)
        {
            var items = (await _itemRepository.GetAll()).Where(r => r.RequestId == requestId && r.IsDeleted == false).ToList();
            var itemsDTO = _mapper.Map<List<ResponseReimbursementItemDTO>>(items);
            foreach (var item in itemsDTO)
            {
                item.CategoryName = (await _categoryRepository.Get(item.CategoryId)).Name;
            }
            return itemsDTO;
        }


        public async Task<List<ResponseReimbursementRequestDTO>> MappingReimbursementRequest(List<ReimbursementRequest> requests)
        {
            List<ResponseReimbursementRequestDTO> requestDTOs = new List<ResponseReimbursementRequestDTO>();

            foreach (var request in requests)
            {
                ResponseReimbursementRequestDTO newDTO = new ResponseReimbursementRequestDTO()
                {
                    Id = request.Id,
                    UserId = request.UserId,
                    User = await GetUser(request.UserId),
                    TotalAmount = request.TotalAmount,
                    PolicyId = request.PolicyId,
                    PolicyName = (await _policyRepository.Get(request.PolicyId)).PolicyName,
                    Items = await GetItemsByRequestId(request.Id),
                    Comments = request.Comments,
                    Stage = request.Stage,
                    Status = request.Status,
                    DateTime = request.CreatedAt,


                };

                requestDTOs.Add(newDTO);

            }
            return requestDTOs;

        }

        public async Task<PaginatedResultDTO<ResponseReimbursementRequestDTO>> GetRequestsByStatusAsync(RequestStatus status, int pageNumber, int pageSize)
        {
            try
            {
                var requests = await _repository.GetAll();
                var filter_request = requests.Where(r => r.Status == status && r.IsDeleted == false).ToList();

                var total = requests.Count();
                var pagedItems = requests
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                //List<ResponseReimbursementRequestDTO> requestDTOs = _mapper.Map<List<ResponseReimbursementRequestDTO>>(filter_request);
                List<ResponseReimbursementRequestDTO> requestDTOs = await MappingReimbursementRequest(filter_request);

                foreach (var request in requestDTOs)
                {

                    request.Items = await GetItemsByRequestId(request.Id);


                }

                return new PaginatedResultDTO<ResponseReimbursementRequestDTO>
                {
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalCount = total,
                    TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                    Data = requestDTOs

                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PaginatedResultDTO<ResponseReimbursementRequestDTO>> GetRequestsByPolicyAsync(int policyId, int pageNumber, int pageSize)
        {
            try
            {
                var requests = await _repository.GetAll();
                var filter_request = requests.Where(r => r.PolicyId == policyId && r.IsDeleted == false).ToList();

                var total = requests.Count();
                var pagedItems = filter_request
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                //List<ResponseReimbursementRequestDTO> requestDTOs = _mapper.Map<List<ResponseReimbursementRequestDTO>>(filter_request);
                List<ResponseReimbursementRequestDTO> requestDTOs = await MappingReimbursementRequest(pagedItems);

                foreach (var request in requestDTOs)
                {

                    request.Items = await GetItemsByRequestId(request.Id);


                }

                return new PaginatedResultDTO<ResponseReimbursementRequestDTO>
                {
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalCount = total,
                    TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                    Data = requestDTOs

                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseReimbursementItemDTO> DeleteItem(int itemId)
        {
            var item = await _itemRepository.Get(itemId);
            var itemDTO = _mapper.Map<ResponseReimbursementItemDTO>(item);
            item.IsDeleted = true;
            await _itemRepository.Update(item.Id, item);
            itemDTO.CategoryName = (await _categoryRepository.Get(item.CategoryId)).Name;
            return itemDTO;
        }

        public async Task<SuccessResponseDTO<int>> DeleteRequestAsync(int requestId)
        {
            try
            {
                var request = await _repository.Get(requestId);
                request.IsDeleted = true;
                var items = await GetItemsByRequestId(request.Id);
                foreach (var item in items)
                {
                    await DeleteItem(item.Id);
                }

                var update = await _repository.Update(requestId, request);
                return new SuccessResponseDTO<int>
                {
                    IsSuccess = true,
                    Message = "Deleted",
                    Data = update.Id
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<PaginatedResultDTO<ResponseReimbursementRequestDTO>> GetRequestsByManagerIdAsync(int managerId, int pageNumber, int pageSize)
        {
            try
            {
                var employess = (await _employeeRepository.GetAll()).Where(e => e.ManagerId == managerId).ToList();

                var approvals = await _approvalStageRepository.GetAll();

                List<ResponseEmployeeDTO> result = new List<ResponseEmployeeDTO>();
                foreach (var employee in employess)
                {
                    var newEmployeeDTO = new ResponseEmployeeDTO()
                    {
                        Id = employee.Id,
                        EmployeeId = employee.EmployeeId,
                        Employee = await GetUser(employee.EmployeeId),
                        ManagerId = employee.ManagerId,
                        Manager = await GetUser(employee.ManagerId),
                    };
                    result.Add(newEmployeeDTO);

                }

                //var employees = (await _employeeService.GetEmployeesByManagerIdAsync(managerId, pageNumber, pageSize)).Data;


                List<ResponseReimbursementRequestDTO> requestEmployee = new List<ResponseReimbursementRequestDTO>();
                foreach (var employee in result)
                {
                    //List<ResponseReimbursementRequestDTO> requests = (await GetRequestsByUserIdAsync(employee.EmployeeId, pageNumber, pageSize)).Data;

                    var allrequests = await _repository.GetAll();

                    var requests = allrequests.Where(r => r.UserId == employee.EmployeeId && r.IsDeleted == false).ToList();
                    //  && r.Stage == Stage.Processing

                    List<ResponseReimbursementRequestDTO> requestDTOs = await MappingReimbursementRequest(requests);
                    if (requestDTOs.Count > 0)
                    {
                        foreach (var request in requestDTOs)
                        {
                            request.Items = await GetItemsByRequestId(request.Id);
                            var approval = approvals.FirstOrDefault(a => a.RequestId == request.Id && a.ReviewId == managerId && a.Status == Status.Approved);
                            request.IsApprovedByManager = approval != null;
                            requestEmployee.Add(request);
                        }
                    }

                }
                var total = requestEmployee.Count;
                var pagedItems = requestEmployee
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                //List<ResponseReimbursementRequestDTO> requestDTOs = _mapper.Map<List<ResponseReimbursementRequestDTO>>(filter_request);


                return new PaginatedResultDTO<ResponseReimbursementRequestDTO>
                {
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalCount = total,
                    TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                    Data = pagedItems

                };

            }
            catch (Exception ex)
            {
                throw new NotFoundException(ex.Message);
            }
        }

        public async Task<ResponseReimbursementItemDTO> AddItemService(ReimbursementItemDTO itemDTO)
        {
            var reimbursementItems = await MappingItems(itemDTO);
            var item = await _itemRepository.Add(reimbursementItems);
            var responseItem = _mapper.Map<ResponseReimbursementItemDTO>(item);
            responseItem.CategoryName = (await _categoryRepository.Get(responseItem.CategoryId)).Name;
            return responseItem;
        }

        public async Task<ReimbursementItem> MappingItems(ReimbursementItemDTO itemDto)
        {
            return new ReimbursementItem()
            {
                RequestId = itemDto.RequestId,
                Amount = itemDto.Amount,
                CategoryId = itemDto.CategoryId,
                Description = itemDto.Description,
                receiptFile = await SaveFileAsync(itemDto.receiptFile)
            };



        }
        public async Task<string> SaveFileAsync(IFormFile file)
        {


            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(_uploadFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return uniqueFileName;
        }

        public async Task<PaginatedResultDTO<ResponseReimbursementRequestDTO>> GetAllRequest(int pageNumber, int pageSize)
        {
            try
            {


                var requests = await _repository.GetAll();
                var filter_request = requests.Where(r => r.IsDeleted == false).ToList();

                var total = filter_request.Count;
                //List<ResponseReimbursementRequestDTO> requestDTOs = _mapper.Map<List<ResponseReimbursementRequestDTO>>(filter_request);
                List<ResponseReimbursementRequestDTO> requestDTOs = await MappingReimbursementRequest(filter_request);

                // foreach (var request in requestDTOs)
                // {
                //     request.Items = await GetItemsByRequestId(request.Id);
                // }

                var pagedItems = requestDTOs
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return new PaginatedResultDTO<ResponseReimbursementRequestDTO>
                {
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalCount = total,
                    TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                    Data = pagedItems

                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DashboardResponseDTO> GetDashboard()
        {
            try
            {
                var approved = (await _repository.GetAll()).Where(r => r.Status == RequestStatus.Passed && r.IsDeleted == false).Count();

                var pending = (await _repository.GetAll()).Where(r => r.Status == RequestStatus.Pending && r.IsDeleted == false).Count();

                var employee = (await _userRepository.GetAll()).Where(e => e.IsDeleted == false).Count();

                var rejected = (await _repository.GetAll()).Where(r => r.Status == RequestStatus.Rejected && r.IsDeleted == false).Count();

                var recents = (await _repository.GetAll()).Where(r => r.IsDeleted == false).OrderByDescending(r => r.UpdatedAt);

                // var manager = (await _userRepository.GetAll()).Where(r => r.IsManager == true && r.IsDeleted == false).Count();

                List<RecentActivityDTO> recentActivity = new List<RecentActivityDTO>();

                foreach (var recent in recents)
                {
                    RecentActivityDTO recentActivityDTO = new RecentActivityDTO()
                    {
                        Amount = recent.TotalAmount,
                        Date = recent.UpdatedAt,
                        EmployeeName = (await _userRepository.Get(recent.UserId)).UserName,
                        Status = recent.Status.ToString(),
                        Id = recent.Id

                    };
                    recentActivity.Add(recentActivityDTO);
                }

                DashboardResponseDTO reposnseDTO = new DashboardResponseDTO()
                {
                    ApprovedRequests = approved,
                    EmployeesCount = employee,
                    PendingRequests = pending,
                    RejectedRequests = rejected,
                    RecentActivities = recentActivity,
                    TotalRequests = (await _repository.GetAll()).Where(r => r.IsDeleted == false).Count()

                };

                return reposnseDTO;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
