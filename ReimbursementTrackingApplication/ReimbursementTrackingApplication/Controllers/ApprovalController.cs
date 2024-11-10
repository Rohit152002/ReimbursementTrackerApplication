using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models.DTOs;
using ReimbursementTrackingApplication.Services;

namespace ReimbursementTrackingApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApprovalController : ControllerBase
    {
        private readonly IApprovalService _approvalService;
        public ApprovalController(IApprovalService approvalService)
        {
            _approvalService = approvalService;
        }

        //Task<SuccessResponseDTO<ResponseApprovalStageDTO>> GetApprovalByIdAsync(int approvalId);

        [HttpGet("{id}")]
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

        //Task<PaginatedResultDTO<ResponseApprovalStageDTO>> GetApprovalsByRequestIdAsync(int requestId, int pageNumber, int pageSize);
        [HttpGet("request/{requestId}")]
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

        //Task<PaginatedResultDTO<ResponseApprovalStageDTO>> GetApprovalsByReviewerIdAsync(int reviewerId, int pageNumber, int pageSize);

        [HttpGet("request/{reviewId}")]
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


        //Task<PaginatedResultDTO<ResponseApprovalStageDTO>> GetHrPendingApprovalsAsync(int pageNumber, int pageSize);
        [HttpGet("hr")]
        [Authorize("Role=HR")]
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


        //Task<PaginatedResultDTO<ResponseApprovalStageDTO>> GetFinancePendingApprovalsAsync(int pageNumber, int pageSize);
        [HttpGet("finace")]
        [Authorize("Role=Finance")]
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

        //Task<SuccessResponseDTO<int>> ApproveRequestAsync(ApprovalStageDTO approval);
        [HttpPost("approve")]
        public async Task<ActionResult<PaginatedResultDTO<ResponseApprovalStageDTO>>> ApproveRequest(ApprovalStageDTO approval)
        {
            try
            {
                var result = await _approvalService.ApproveRequestAsync(approval);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }
        //Task<SuccessResponseDTO<int>> RejectRequestAsync(ApprovalStageDTO approval);
        [HttpPost("reject")]
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
