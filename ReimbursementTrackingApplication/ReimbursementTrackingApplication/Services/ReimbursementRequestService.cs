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
        private readonly IRepository<int,ReimbursementRequest> _repository;
        private readonly IRepository<int,Policy> _policyRepository;

        private readonly string _uploadFolder;
        private readonly IRepository<int, ReimbursementItem> _itemRepository;

        private readonly IRepository<int,Employee> _employeeRepository;
        private readonly IRepository<int, ExpenseCategory> _categoryRepository;
        private readonly IRepository<int, User> _userRepository;

        private readonly IMapper _mapper;
        public ReimbursementRequestService(
            IRepository<int, ReimbursementRequest> repository,
            IRepository<int, Policy> policyRepository,
            IMapper mapper,
             string uploadFolder,
        IRepository<int, ReimbursementItem> itemRepository,
            IRepository<int, Employee> employeeRepository,
            IRepository<int, ExpenseCategory> categoryRepository,
            IRepository<int, User> userRepository)
        {
            _userRepository = userRepository;
            _repository = repository;
            _policyRepository = policyRepository;

            _mapper=mapper;
            _uploadFolder = uploadFolder;

            _itemRepository = itemRepository;
            _employeeRepository = employeeRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<SuccessResponseDTO<ResponseReimbursementRequestDTO>> SubmitRequestAsync(CreateReimbursementRequestDTO requestDto)
        {
            try
            {
                var request = MapRequest(requestDto);

                var requestAdded = await _repository.Add(request);

                var items = (await AddItemsAsync(requestDto.Items,requestAdded.Id)).ToList();


                request.Items = _mapper.Map<List<ReimbursementItem>>(items);


                var responseRequest = await CreateResponseDTO(requestAdded, items, requestDto.UserId, requestDto.PolicyId);


                return new SuccessResponseDTO<ResponseReimbursementRequestDTO>
                {
                    IsSuccess = true,
                    Message = "Request Created Successfully",
                    Data = responseRequest
                };

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        private async Task<List<ResponseReimbursementItemDTO>> AddItemsAsync(IEnumerable<RequestITemDTO> itemDTOs, int id)
        {
            List<ResponseReimbursementItemDTO> responseItemDTOs= new List<ResponseReimbursementItemDTO>();

            List<ReimbursementItemDTO> items = new List<ReimbursementItemDTO>();
            foreach(var item in itemDTOs)
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
            foreach(var item in requestDto.Items)
            {
                totalsum += item.Amount;
            }
            return new ReimbursementRequest {
                UserId= requestDto.UserId,
                PolicyId= requestDto.PolicyId,
                TotalAmount= totalsum,
                Comments= requestDto.Comments,
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

                requestDTO.PolicyName= (await _policyRepository.Get(request.PolicyId)).PolicyName;
                requestDTO.Items = await GetItemsByRequestId(requestDTO.Id);
                requestDTO.User=  await GetUser(request.UserId);
                return new SuccessResponseDTO<ResponseReimbursementRequestDTO>()
                {
                    IsSuccess=true,
                    Message="Data Fetch Successfully",
                    Data=requestDTO

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
                var filter_request = requests.Where(r => r.UserId == userId && r.IsDeleted==false).ToList();

                var total = filter_request.Count();
                var pagedItems = filter_request
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                //List<ResponseReimbursementRequestDTO> requestDTOs = _mapper.Map<List<ResponseReimbursementRequestDTO>>(filter_request);
                List<ResponseReimbursementRequestDTO> requestDTOs = await MappingReimbursementRequest(filter_request);

                foreach(var request in requestDTOs)
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
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ResponseReimbursementItemDTO>> GetItemsByRequestId(int requestId)
        {
            var items = (await _itemRepository.GetAll()).Where(r => r.RequestId == requestId && r.IsDeleted == false).ToList();
            var itemsDTO = _mapper.Map<List<ResponseReimbursementItemDTO>>(items);
            foreach(var item in itemsDTO)
            {
                item.CategoryName = (await _categoryRepository.Get(item.CategoryId)).Name;
            }
            return itemsDTO;
        }


        public async Task<List<ResponseReimbursementRequestDTO> > MappingReimbursementRequest(List<ReimbursementRequest> requests)
        {
            List<ResponseReimbursementRequestDTO> requestDTOs = new List<ResponseReimbursementRequestDTO>();

            foreach (var request in requests)
            {
                ResponseReimbursementRequestDTO newDTO = new ResponseReimbursementRequestDTO()
                {
                    Id = request.Id,
                    UserId = request.UserId,
                    User = await GetUser(request.UserId),
                    PolicyId = request.PolicyId,
                    PolicyName = (await _policyRepository.Get(request.PolicyId)).PolicyName,
                    Items = await GetItemsByRequestId(request.Id),
                    Comments = request.Comments

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
            catch(Exception ex)
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

        public async  Task<ResponseReimbursementItemDTO> DeleteItem(int itemId)
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
                return new SuccessResponseDTO<int> {
                    IsSuccess = true,
                    Message="Deleted",
                    Data=update.Id
                };
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<PaginatedResultDTO<ResponseReimbursementRequestDTO>> GetRequestsByManagerIdAsync(int managerId, int pageNumber, int pageSize)
        {
            try
            {
                var employess = (await _employeeRepository.GetAll()).Where(e => e.ManagerId == managerId).ToList();

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

                    List<ResponseReimbursementRequestDTO> requestDTOs = await MappingReimbursementRequest(requests);
                    if(requestDTOs.Count > 0)
                    {
                            foreach (var request in requestDTOs)
                            {
                                request.Items = await GetItemsByRequestId(request.Id);
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
            var reimbursementItems =  await MappingItems(itemDTO);
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
            var filter_request = requests.Where(r=>r.IsDeleted == false).ToList();

            var total = filter_request.Count;
            var pagedItems = filter_request
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
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
    }
}
    }
}
