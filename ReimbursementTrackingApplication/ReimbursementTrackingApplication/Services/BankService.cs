using AutoMapper;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Models.DTOs;

namespace ReimbursementTrackingApplication.Services
{
    public class BankService : IBankService
    {
        private readonly IRepository<int, BankAccount> _repository;
        private readonly IRepository<int, User> _userRepository;
        private readonly IMapper _mapper;
        public BankService(IRepository<int, User> userRepository, IRepository<int, BankAccount> repository, IMapper mapper)
        {
            _userRepository = userRepository;
            _repository = repository;
            _mapper = mapper;
        }


        public async Task<SuccessResponseDTO<ResponseBankDTO>> AddBankAccountAsync(BankDTO bankAccount)
        {
            try
            {
                var bank = _mapper.Map<BankAccount>(bankAccount);
                var addedBank = await _repository.Add(bank);
                var user = await _userRepository.Get(bank.UserId);
                var userDTO = _mapper.Map<UserDTO>(user);

                var response = new ResponseBankDTO()
                {
                    Id = addedBank.Id,
                    AccNo = addedBank.AccNo,
                    BranchAddress = addedBank.BranchAddress,
                    BranchName = addedBank.BranchName,
                    IFSCCode = addedBank.IFSCCode,
                    User = userDTO,
                    UserId = addedBank.UserId,
                };

                return new SuccessResponseDTO<ResponseBankDTO>()
                {
                    Data = response,
                    IsSuccess = true,
                    Message = "Added Succesfull"
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<SuccessResponseDTO<int>> DeleteBankAccountAsync(int id)
        {
            try
            {
                var bank = await _repository.Get(id);
                bank.IsDeleted = true;
                var update = await _repository.Update(id, bank);
                return new SuccessResponseDTO<int>()
                {
                    Data = update.Id,
                    IsSuccess = true,
                    Message = "Delete Successfully"
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PaginatedResultDTO<ResponseBankDTO>> GetAllBankAccountsAsync(int pageNumber, int pageSize)
        {
            try
            {
                var banks = (await _repository.GetAll()).ToList();

                //var banksDTO = _mapper.Map<List<ResponseBankDTO>>(banks);

                List<ResponseBankDTO> banksDTO = new List<ResponseBankDTO>();

                foreach (var bank in banks)
                {
                    var user = await _userRepository.Get(bank.UserId);
                    var userDTO = _mapper.Map<UserDTO>(user);

                    ResponseBankDTO responseBankDTO = new ResponseBankDTO()
                    {
                        AccNo = bank.AccNo,
                        User = userDTO,
                        BranchAddress = bank.BranchAddress,
                        BranchName = bank.BranchName,
                        Id = bank.Id,
                        IFSCCode = bank.IFSCCode,
                        UserId = bank.UserId,

                    };
                    banksDTO.Add(responseBankDTO);
                }
                var total = banksDTO.Count;
                var pagedItems = banksDTO
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return new PaginatedResultDTO<ResponseBankDTO>
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

        public async Task<SuccessResponseDTO<ResponseBankDTO>> GetBankAccountByIdAsync(int id)
        {
            try
            {
                var addedBank = await _repository.Get(id);
                var user = await _userRepository.Get(addedBank.UserId);
                var userDTO = _mapper.Map<UserDTO>(user);

                var response = new ResponseBankDTO()
                {
                    Id = addedBank.Id,
                    AccNo = addedBank.AccNo,
                    BranchAddress = addedBank.BranchAddress,
                    BranchName = addedBank.BranchName,
                    IFSCCode = addedBank.IFSCCode,
                    User = userDTO,
                    UserId = addedBank.UserId,
                };

                return new SuccessResponseDTO<ResponseBankDTO>()
                {
                    Data = response,
                    IsSuccess = true,
                    Message = "Fetch Succesfull"
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<SuccessResponseDTO<ResponseBankDTO>> GetBankAccountByUserIdAsync(int userId)
        {
            try
            {
                var banks = await _repository.GetAll();
                var bank = banks.FirstOrDefault(x => x.UserId == userId);
                if (bank == null)
                {
                    throw new Exception("Bank Not Found for this user");
                }
                var bankDTO = _mapper.Map<BankDTO>(bank);
                var user = await _userRepository.Get(bank.UserId);
                var userDTO = _mapper.Map<UserDTO>(user);
                ResponseBankDTO bankResponse = new ResponseBankDTO()
                {
                    Id = bank.Id,
                    AccNo = bank.AccNo,
                    BranchAddress = bank.BranchAddress,
                    BranchName = bank.BranchName,
                    IFSCCode = bank.IFSCCode,
                    User = userDTO,
                    UserId = bank.UserId,
                };
                return new SuccessResponseDTO<ResponseBankDTO>()
                {
                    IsSuccess = true,
                    Message = "Fetch Successfully",
                    Data = bankResponse
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<SuccessResponseDTO<ResponseBankDTO>> UpdateBankAccountAsync(int id, BankDTO bankAccount)
        {
            try
            {

                var bank = _mapper.Map<BankAccount>(bankAccount);
                //bank.Id = id;
                var updatebank = await _repository.Update(id, bank);
                var user = await _userRepository.Get(updatebank.UserId);
                var userDTO = _mapper.Map<UserDTO>(user);

                var response = new ResponseBankDTO()
                {
                    Id = updatebank.Id,
                    AccNo = updatebank.AccNo,
                    BranchAddress = updatebank.BranchAddress,
                    BranchName = updatebank.BranchName,
                    IFSCCode = updatebank.IFSCCode,
                    User = userDTO,
                    UserId = updatebank.UserId,
                };

                return new SuccessResponseDTO<ResponseBankDTO>()
                {
                    Data = response,
                    IsSuccess = true,
                    Message = "Added Succesfull"
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
