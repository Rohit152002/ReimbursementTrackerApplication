using AutoMapper;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Models.DTOs;
using ReimbursementTrackingApplication.Repositories;

namespace ReimbursementTrackingApplication.Services
{
    public class ExpenseCategoryService : IExpenseCategoryService
    {
        private readonly IRepository<int,ExpenseCategory> _repository;
        private readonly IMapper _mapper;

        public ExpenseCategoryService(IRepository<int, ExpenseCategory> repository,IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<SuccessResponseDTO<int>> AddCategoryAsync(CreateCategoryDTO categoryDto)
        {
            try
            {
                var category= _mapper.Map<ExpenseCategory>(categoryDto);
                var categoryData = await _repository.Add(category);
                return new SuccessResponseDTO<int> { 
                IsSuccess=true,
                Message="Add Category Successfull",
                Data=categoryData.Id};
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<SuccessResponseDTO<int>> DeleteCategoryAsync(int categoryId)
        {
            try
            {
       
                var categoryData = await _repository.Get(categoryId);
                categoryData.IsDeleted = true;

                var updateData = await _repository.Update(categoryId, categoryData);
                return new SuccessResponseDTO<int>
                {
                    IsSuccess = true,
                    Message = "Delete Category Successfull",
                    Data = updateData.Id
                };
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<SuccessResponseDTO<int>> UpdateCategoryAsync(int categoryId, CreateCategoryDTO categoryDto)
        {
            try
            {
                var category = _mapper.Map<ExpenseCategory>(categoryDto);

                var categoryData = await _repository.Update(categoryId,category);
                return new SuccessResponseDTO<int>
                {
                    IsSuccess = true,
                    Message = "Update Category Successfull",
                    Data = categoryData.Id
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<PaginatedResultDTO<ExpenseCategoryDTO>> GetAllCategoriesAsync(int pageNumber, int pageSize)
        {
            try
            {

            var categorys= (await _repository.GetAll()).Where(c=>c.IsDeleted == false);
            var total = categorys.Count();

            var pagedItems = categorys
                 .Skip((pageNumber - 1) * pageSize)
                 .Take(pageSize)
                 .ToList();

            var categoryDTOs = _mapper.Map<List<ExpenseCategoryDTO>>(pagedItems);
            return new PaginatedResultDTO<ExpenseCategoryDTO>
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                Data = categoryDTOs

            };
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<SuccessResponseDTO<ExpenseCategoryDTO>> GetCategoryByIdAsync(int categoryId)
        {
            try
            {

                var categoryData = await _repository.Get(categoryId);
                var categoryDataDTO = _mapper.Map<ExpenseCategoryDTO>(categoryData);
       
                return new SuccessResponseDTO<ExpenseCategoryDTO>
                {
                    IsSuccess = true,
                    Message = "Delete Category Successfull",
                    Data = categoryDataDTO
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
