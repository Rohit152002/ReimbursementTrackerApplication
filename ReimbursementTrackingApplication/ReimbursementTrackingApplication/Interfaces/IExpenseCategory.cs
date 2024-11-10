using ReimbursementTrackingApplication.Models.DTOs;

namespace ReimbursementTrackingApplication.Interfaces
{
    public interface IExpenseCategoryService
    {
        Task<SuccessResponseDTO<ExpenseCategoryDTO>> GetCategoryByIdAsync(int categoryId);
        Task<PaginatedResultDTO<ExpenseCategoryDTO>> GetAllCategoriesAsync(int pageNumber, int pageSize);
        Task<SuccessResponseDTO<int>> AddCategoryAsync(CreateCategoryDTO categoryDto);
        Task<SuccessResponseDTO<int>> UpdateCategoryAsync(int categoryId, CreateCategoryDTO categoryDto);
        Task<SuccessResponseDTO<int>> DeleteCategoryAsync(int categoryId);
    }

}
