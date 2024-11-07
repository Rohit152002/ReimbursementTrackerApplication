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
        private readonly IUserServices _userService;
        private readonly ReimbursementItemService _itemService;

        private readonly IMapper _mapper;
        public ReimbursementRequestService(IRepository<int, ReimbursementRequest> repository, IRepository<int, Policy> policyRepository, IUserServices userService,IMapper mapper,ReimbursementItemService itemService)
        {
            _repository = repository;
            _policyRepository = policyRepository;
            _userService = userService;
            _mapper=mapper;
            _itemService = itemService;
        }

        public async Task<SuccessResponseDTO<ResponseReimbursementRequestDTO>> SubmitRequestAsync(CreateReimbursementRequestDTO requestDto)
        {
            try
            {
                var request = MapRequest(requestDto);

                var requestAdded = await _repository.Add(request);

                var items = (await AddItemsAsync(requestDto.Items)).ToList();


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
     

        private async Task<List<ResponseReimbursementItemDTO>> AddItemsAsync(IEnumerable<ReimbursementItemDTO> itemDTOs)
        {
            var itemTasks = itemDTOs.Select(async item =>
            {
                return (await _itemService.AddItemAsync(item)).Data;
            });
            return (await Task.WhenAll(itemTasks)).ToList();
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
            responseRequest.User = await _userService.GetUserProfile(userId);
            responseRequest.PolicyName = (await _policyRepository.Get(policyId)).PolicyName;

            return responseRequest;
        }

        public async Task<SuccessResponseDTO<ResponseReimbursementRequestDTO>> GetRequestByIdAsync(int requestId)
        {
           try
            {
                ReimbursementRequest request = await _repository.Get(requestId);
               
                ResponseReimbursementRequestDTO requestDTO = _mapper.Map<ResponseReimbursementRequestDTO>(request);
                requestDTO.PolicyName= (await _policyRepository.Get(request.PolicyId)).PolicyName;
                requestDTO.User=  await _userService.GetUserProfile(request.UserId);
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

                var total = requests.Count();
                var pagedItems = requests
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                //List<ResponseReimbursementRequestDTO> requestDTOs = _mapper.Map<List<ResponseReimbursementRequestDTO>>(filter_request);
                List<ResponseReimbursementRequestDTO> requestDTOs = await MappingReimbursementRequest(filter_request);
               
                foreach(var request in requestDTOs)
                {
                    var items = await _itemService.GetItemsByRequestIdAsync(request.Id,1,100);
                    request.Items = items.Data;
                   
                    
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

        public async Task<List<ResponseReimbursementRequestDTO> > MappingReimbursementRequest(List<ReimbursementRequest> requests)
        {
            List<ResponseReimbursementRequestDTO> requestDTOs = new List<ResponseReimbursementRequestDTO>();

            foreach (var request in requests)
            {
                ResponseReimbursementRequestDTO newDTO = new ResponseReimbursementRequestDTO()
                {
                    Id = request.Id,
                    UserId = request.UserId,
                    User = await _userService.GetUserProfile(request.UserId),
                    PolicyId = request.PolicyId,
                    PolicyName = (await _policyRepository.Get(request.PolicyId)).PolicyName,
                    Items =( await _itemService.GetItemsByRequestIdAsync(request.Id,1,100)).Data
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
                    var items = await _itemService.GetItemsByRequestIdAsync(request.Id, 1, 100);
                    request.Items = items.Data;


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
                var pagedItems = requests
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                //List<ResponseReimbursementRequestDTO> requestDTOs = _mapper.Map<List<ResponseReimbursementRequestDTO>>(filter_request);
                List<ResponseReimbursementRequestDTO> requestDTOs = await MappingReimbursementRequest(filter_request);
          
                foreach (var request in requestDTOs)
                {
                    var items = await _itemService.GetItemsByRequestIdAsync(request.Id, 1, 100);
                    request.Items = items.Data;


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

        public async Task<SuccessResponseDTO<int>> DeleteRequestAsync(int requestId)
        {
            try
            {
                var request = await _repository.Get(requestId);
                request.IsDeleted = true;
                var items = await _itemService.GetItemsByRequestIdAsync(requestId, 1, 100);
                foreach(var item in items.Data)
                {
                    await _itemService.DeleteItemAsync(item.Id);
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
    }
}
