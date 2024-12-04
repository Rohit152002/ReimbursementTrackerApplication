using AutoMapper;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Models.DTOs;
using ReimbursementTrackingApplication.Repositories;

namespace ReimbursementTrackingApplication.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IRepository<int, Payment> _repository;

        //private readonly ReimbursementRequestService _requestService;

        private readonly IRepository<int, ReimbursementRequest> _reimbursementRequestRepository;
        private readonly IRepository<int, Policy> _policyRepository;
        private readonly IRepository<int, User> _userRepository;
        private readonly IRepository<int, ExpenseCategory> _categoryRepository;
        private readonly IRepository<int, ReimbursementItem> _itemRepository;
        private readonly IMapper _mapper;


        public PaymentService(
            IRepository<int, Payment> repository,
            //ReimbursementRequestService requestService,
            IRepository<int, ReimbursementRequest> reimbursementRequestRepository,
            IMapper mapper,
            IRepository<int, Policy> policyRepository,
            IRepository<int, User> userRepository,
            IRepository<int, ReimbursementItem> itemRepository,
            IRepository<int, ExpenseCategory> categoryRepository
            )
        {
            _repository = repository;
            //_requestService = requestService;
            _reimbursementRequestRepository = reimbursementRequestRepository;
            _mapper = mapper;
            _policyRepository = policyRepository;
            _userRepository = userRepository;
            _itemRepository = itemRepository;
            _categoryRepository = categoryRepository;

        }
        public async Task<SuccessResponseDTO<int>> DeletePaymentAsync(int paymentId)
        {
            try
            {
                var payment = await _repository.Get(paymentId);
                payment.IsDeleted = true;
                var updatePayment = await _repository.Update(paymentId, payment);
                return new SuccessResponseDTO<int>()
                {
                    IsSuccess = true,
                    Message = "Deleted Successfully",
                    Data = updatePayment.Id
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

        public async Task<ResponseReimbursementRequestDTO> GetRequestbyId(int requestId)
        {
            ReimbursementRequest request = await _reimbursementRequestRepository.Get(requestId);

            ResponseReimbursementRequestDTO requestDTO = _mapper.Map<ResponseReimbursementRequestDTO>(request);
            requestDTO.Items = await GetItemsByRequestId(requestId);

            requestDTO.PolicyName = (await _policyRepository.Get(request.PolicyId)).PolicyName;

            requestDTO.User = await GetUser(request.UserId);
            requestDTO.IsApprovedByFinance = true;
            requestDTO.IsApprovedByManager = true;
            requestDTO.IsApprovedByHr = true;

            return requestDTO;
        }
        public async Task<UserDTO> GetUser(int id)
        {
            var user = await _userRepository.Get(id);
            var userDTO = _mapper.Map<UserDTO>(user);
            return userDTO;

        }

        public async Task<PaginatedResultDTO<ResponsePayment>> GetAllPaymentsAsync(int pageNumber, int pageSize)
        {
            try
            {
                var payments = (await _repository.GetAll()).Where(p => p.IsDeleted == false);


                List<ResponsePayment> responses = new List<ResponsePayment>();

                foreach (var payment in payments)
                {
                    ResponseReimbursementRequestDTO request = await GetRequestbyId(payment.RequestId);

                    ResponsePayment responsePayment = new ResponsePayment();
                    responsePayment.Id = payment.Id;
                    responsePayment.RequestId = request.Id;
                    responsePayment.Request = request;
                    responsePayment.PaymentDate = payment.PaymentDate;
                    responsePayment.PaymentStatus = payment.PaymentStatus;

                    responses.Add(responsePayment);
                }

                var total = responses.Count();
                var pagedItems = responses
               .Skip((pageNumber - 1) * pageSize)
               .Take(pageSize)
               .ToList();


                return new PaginatedResultDTO<ResponsePayment>
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

        public async Task<SuccessResponseDTO<ResponsePayment>> GetPaymentByRequestIdAsync(int requestId)
        {
            try
            {

                var payment = (await _repository.GetAll()).FirstOrDefault(p => p.RequestId == requestId);

                ResponseReimbursementRequestDTO request = await GetRequestbyId(payment.RequestId);

                ResponsePayment responsePayment = new ResponsePayment()
                {
                    Id = payment.Id,
                    RequestId = request.Id,
                    Request = request,
                    AmountPaid = request.TotalAmount,
                    PaymentMethod = payment.PaymentMethod,
                    PaymentDate = payment.PaymentDate,
                    PaymentStatus = payment.PaymentStatus,
                };



                return new SuccessResponseDTO<ResponsePayment>()
                {
                    IsSuccess = true,
                    Message = "Fetch Successfully",
                    Data = responsePayment
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PaginatedResultDTO<ResponsePayment>> GetPaymentsByUserIdAsync(int userId, int pageNumber, int pageSize)
        {
            try
            {
                var requests = await _reimbursementRequestRepository.GetAll();
                var filter_request = requests.Where(r => r.UserId == userId && r.IsDeleted == false).ToList();




                var allPayments = (await _repository.GetAll()).Where(p => p.IsDeleted == false).ToList();

                List<Payment> payments = new List<Payment>();
                foreach (var request in filter_request)
                {
                    if (request.UserId == userId)
                    {
                        Payment payment = allPayments.FirstOrDefault(p => p.RequestId == request.Id);
                        if (payment != null)
                        {
                            payments.Add(payment);

                        }
                    }

                }


                List<ResponsePayment> responses = new List<ResponsePayment>();

                foreach (var payment in payments)
                {
                    ResponseReimbursementRequestDTO request = await GetRequestbyId(payment.RequestId);

                    ResponsePayment responsePayment = new ResponsePayment();
                    responsePayment.Id = payment.Id;
                    responsePayment.RequestId = request.Id;
                    responsePayment.Request = request;
                    responsePayment.PaymentDate = payment.PaymentDate;
                    responsePayment.PaymentStatus = payment.PaymentStatus;
                    responsePayment.PaymentMethod = payment.PaymentMethod;
                    responsePayment.AmountPaid = payment.AmountPaid;

                    responses.Add(responsePayment);
                }

                var total = responses.Count();
                var pagedItems = responses
               .Skip((pageNumber - 1) * pageSize)
               .Take(pageSize)
               .ToList();


                return new PaginatedResultDTO<ResponsePayment>
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

        public async Task<SuccessResponseDTO<ResponsePayment>> ProcessPaymentAsync(PaymentDTO payment)
        {
            try
            {

                var getpayment = await _repository.Get(payment.Id);
                getpayment.PaymentMethod = payment.PaymentMethod;
                getpayment.PaymentStatus = PaymentStatus.Paid;
                getpayment.PaymentDate = DateTime.Now;
                var updatePayment = await _repository.Update(payment.Id, getpayment);

                ResponseReimbursementRequestDTO request = await GetRequestbyId(getpayment.RequestId);


                ResponsePayment responsePayment = new ResponsePayment()
                {
                    Id = payment.Id,
                    RequestId = getpayment.Id,
                    Request = request,
                    PaymentDate = getpayment.PaymentDate,
                    PaymentStatus = getpayment.PaymentStatus,
                };



                return new SuccessResponseDTO<ResponsePayment>()
                {
                    IsSuccess = true,
                    Message = "Payment Successfully",
                    Data = responsePayment
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}
