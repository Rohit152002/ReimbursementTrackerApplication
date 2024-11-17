using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Models.DTOs;
using ReimbursementTrackingApplication.Services;

namespace ReimbursementTrackingApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
     [EnableCors("AllowAll")]
    public class ApprovalController : ControllerBase
    {
        private readonly IApprovalService _approvalService;
        public ApprovalController(IApprovalService approvalService)
        {
            _approvalService = approvalService;
        }


        [HttpGet("{id}")]
        [Authorize(Roles = "Finance,HR,Admin")]
        public async Task<ActionResult<SuccessResponseDTO<ResponseApprovalStageDTO>>> GetApprovalById(int id)
        {
            try
            {
                var result = await _approvalService.GetApprovalByIdAsync(id);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }

        [HttpGet("request/{requestId}")]
        [Authorize(Roles = "Finance,HR,Admin")]
        public async Task<ActionResult<SuccessResponseDTO<ResponseApprovalStageDTO>>> GetApprovalByRequestId(int requestId,int pageNumber =1 , int pageSize =10)
        {
            try
            {
                var result = await _approvalService.GetApprovalsByRequestIdAsync(requestId,pageNumber,pageSize);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }


        [HttpGet("request/review/{reviewId}")]
        [Authorize(Roles = "Fance,HR,Admin")]
        public async Task<ActionResult<SuccessResponseDTO<ResponseApprovalStageDTO>>> GetApprovalByReviewId(int reviewId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var result = await _approvalService.GetApprovalsByReviewerIdAsync(reviewId, pageNumber, pageSize);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }


        [HttpGet("hr")]
        [Authorize(Roles = "HR,Admin")]
        public async Task<ActionResult<PaginatedResultDTO<ResponseApprovalStageDTO>>> GetHrPendingApproval(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var result = await _approvalService.GetHrPendingApprovalsAsync( pageNumber, pageSize);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }



        [HttpGet("finance")]
        [Authorize(Roles = "Finance,Admin")]
        public async Task<ActionResult<PaginatedResultDTO<ResponseApprovalStageDTO>>> GetFinacePendingApproval(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var result = await _approvalService.GetFinancePendingApprovalsAsync(pageNumber, pageSize);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }

        [HttpPost("approve")]
        [Authorize]
        public async Task<ActionResult<PaginatedResultDTO<ResponseApprovalStageDTO>>> ApproveRequest(ApprovalStageDTO approval)
        {
            try
            {
                var result = await _approvalService.ApproveRequestAsync(approval);
                return Ok(result);

            }
            catch(UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorResponseDTO(){  ErrorMessage = ex.Message, ErrorNumber=StatusCodes.Status401Unauthorized});
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }
        [HttpPost("reject")]
        [Authorize]
        public async Task<ActionResult<PaginatedResultDTO<ResponseApprovalStageDTO>>> RejectRequest(ApprovalStageDTO approval)
        {
            try
            {
                var result = await _approvalService.RejectRequestAsync(approval);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }
    }
}
