using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models.DTOs;
using ReimbursementTrackingApplication.Services;

namespace ReimbursementTrackingApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
     [EnableCors("AllowAll")]
    public class CategoryController : ControllerBase
    {
        private readonly IExpenseCategoryService _expenseCategoryService;
        public CategoryController(IExpenseCategoryService expensecategoryService)
        {
            _expenseCategoryService = expensecategoryService;
        }

        [HttpGet("{id}")]
    [Authorize]
        public async Task<ActionResult<SuccessResponseDTO<ExpenseCategoryDTO>>> GetCategoryByid(int id)
        {
            try
            {

                var category = await _expenseCategoryService.GetCategoryByIdAsync(id);
                return Ok(category);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }


        }
        //Task<PaginatedResultDTO<ResponsePolicyDTO>> GetAllPolicesAsync(int pageNumber, int pageSize);
        [HttpGet("all")]
    [Authorize]
        public async Task<ActionResult<PaginatedResultDTO<ExpenseCategoryDTO>>> GetAllCategories(int pageNumber = 1, int pageSize = 10)
        {
            try
            {

                var categories = await _expenseCategoryService.GetAllCategoriesAsync(pageNumber, pageSize);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });
                //throw new Exception("Collection is Empty");

            }


        }
        //Task<SuccessResponseDTO<int>> AddPolicyAsync(CreatePolicyDTO policyDTO);
        [HttpPost]
    [Authorize(Roles ="Admin")]
        public async Task<ActionResult<SuccessResponseDTO<int>>> AddCategories(CreateCategoryDTO categoryDTO)
        {
            try
            {

                var result = await _expenseCategoryService.AddCategoryAsync(categoryDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }
        //Task<SuccessResponseDTO<int>> UpdatePolicyAsync(int categoryId, CreatePolicyDTO policyDTO);
        [HttpPut("{id}")]
    [Authorize(Roles ="Admin")]
        public async Task<ActionResult<SuccessResponseDTO<int>>> UpdateCategory(int id, CreateCategoryDTO categoryDTO)
        {
            try
            {

                var result = await _expenseCategoryService.UpdateCategoryAsync(id, categoryDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }

        //Task<SuccessResponseDTO<int>> DeletePolicyAsync(int policyId);

        [HttpDelete("{id}")]
    [Authorize(Roles ="Admin")]
        public async Task<ActionResult<SuccessResponseDTO<int>>> Delete(int id)
        {
            try
            {

                var result = await _expenseCategoryService.DeleteCategoryAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }
    }
}
