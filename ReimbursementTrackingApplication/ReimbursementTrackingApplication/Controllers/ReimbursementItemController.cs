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
    public class ReimbursementItemController : ControllerBase
    {
        private readonly IReimbursementItemService _reimbursementItemService;
        public ReimbursementItemController(IReimbursementItemService reimbursementItemService)
        {
            _reimbursementItemService = reimbursementItemService;
        }


        //public Task<SuccessResponseDTO<ResponseReimbursementItemDTO>> AddItemAsync(ReimbursementItemDTO itemDto);

        //[HttpPost]
        //[Consumes("multipart/form-data")]
        //public async Task<ActionResult<SuccessResponseDTO<ResponseReimbursementItemDTO>>> AddItemAsync([FromForm] ReimbursementItemDTO itemDto)
        //{
        //    try
        //    {
        //        var result = await _reimbursementItemService.AddItemAsync(itemDto);
        //        return Ok(result);

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

        //    }
        //}
        //public Task<SuccessResponseDTO<ResponseReimbursementItemDTO>> GetItemByIdAsync(int itemId);
        [HttpGet("{itemId}")]
        public async Task<ActionResult<SuccessResponseDTO<ResponseReimbursementItemDTO>>> GetItemById(int itemId)
        {
            try
            {
                var result = await _reimbursementItemService.GetItemByIdAsync(itemId);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }
        //public Task<PaginatedResultDTO<ResponseReimbursementItemDTO>> GetItemsByRequestIdAsync(int requestId, int pageNumber, int pageSize);
        [HttpGet]
        public async Task<ActionResult<PaginatedResultDTO<ResponseReimbursementItemDTO>>> GetItemByRequestId(int requestid,int pagenumber , int pagesize)
        {
            try
            {
                var result = await _reimbursementItemService.GetItemsByRequestIdAsync(requestid,pagenumber,pagesize);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }
        //public Task<SuccessResponseDTO<ResponseReimbursementItemDTO>> UpdateItemAsync(int itemId, ReimbursementItemDTO itemDto);
        //public Task<SuccessResponseDTO<ResponseReimbursementItemDTO>> DeleteItemAsync(int itemId);
        //public Task<PaginatedResultDTO<ResponseReimbursementItemDTO>> GetAllItems(int pageNumber, int pageSize);
    }
}
