using ReimbursementTrackingApplication.Models.DTOs;

namespace ReimbursementTrackingApplication.Interfaces
{
    public interface IReimbursementItemService
    {
        public Task<SuccessResponseDTO<ResponseReimbursementItemDTO>> AddItemAsync(ReimbursementItemDTO itemDto);
        public Task<SuccessResponseDTO<ResponseReimbursementItemDTO>> GetItemByIdAsync(int itemId);
        public Task<PaginatedResultDTO<ResponseReimbursementItemDTO>> GetItemsByRequestIdAsync(int requestId, int pageNumber, int pageSize);
        public Task<SuccessResponseDTO<ResponseReimbursementItemDTO>> UpdateItemAsync(int itemId, ReimbursementItemDTO itemDto);
        public Task<SuccessResponseDTO<ResponseReimbursementItemDTO>> DeleteItemAsync(int itemId);
    }
}
