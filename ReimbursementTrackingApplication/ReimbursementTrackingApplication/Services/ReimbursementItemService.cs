using AutoMapper;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Models.DTOs;
using ReimbursementTrackingApplication.Repositories;

namespace ReimbursementTrackingApplication.Services
{
    public class ReimbursementItemService : IReimbursementItemService
    {
        private readonly IRepository<int, ReimbursementItem> _repository;
        private readonly IRepository<int, ExpenseCategory> _categoryRepository;
        private readonly string _uploadFolder;
        private readonly IMapper _mapper;
        public ReimbursementItemService(IRepository<int, ReimbursementItem> repository,IRepository<int,ExpenseCategory> categoryRepository, string uploadFolder,IMapper mapper)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;   
            _uploadFolder = uploadFolder;
            _mapper = mapper;
            Directory.CreateDirectory(_uploadFolder);
        }
        public async Task<SuccessResponseDTO<ResponseReimbursementItemDTO>> AddItemAsync(ReimbursementItemDTO itemDto)
        {
            try
            {
                var reimbursementItems = await MappingItems(itemDto);
                var item= await _repository.Add(reimbursementItems);
                var reponseItem = _mapper.Map<ResponseReimbursementItemDTO>(item);
                return new SuccessResponseDTO<ResponseReimbursementItemDTO>
                {
                    IsSuccess = true,
                    Message="Item Added Successfully",
                    Data=reponseItem
                };

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
        public async Task<SuccessResponseDTO<ResponseReimbursementItemDTO>> DeleteItemAsync(int itemId)
        {
            try
            {
                var item = await _repository.Get(itemId);
                var itemDTO = _mapper.Map<ResponseReimbursementItemDTO>(item);
                item.IsDeleted = true;
                await _repository.Update(item.Id, item);
                return new SuccessResponseDTO<ResponseReimbursementItemDTO>
                { IsSuccess=true,
                Message="Item Deleted Successfully",
                Data=itemDTO};

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public async Task<SuccessResponseDTO<ResponseReimbursementItemDTO>> GetItemByIdAsync(int itemId)
        {
            try
            {
                 var item = await _repository.Get(itemId);
                var itemDTO = _mapper.Map<ResponseReimbursementItemDTO>(item);
                itemDTO.CategoryName =( await _categoryRepository.Get(itemId)).Name;
                return new SuccessResponseDTO<ResponseReimbursementItemDTO>
                { IsSuccess=true,Message= "Item ResponseReimbursementItemDTO fetch", Data=itemDTO};
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public Task<PaginatedResultDTO<ResponseReimbursementItemDTO>>  GetItemsByRequestIdAsync(int requestId, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<SuccessResponseDTO<ResponseReimbursementItemDTO>> UpdateItemAsync(int itemId, ReimbursementItemDTO itemDto)
        {
            throw new NotImplementedException();
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(_uploadFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return uniqueFileName;
        }
    }
}
