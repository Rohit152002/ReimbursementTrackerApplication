﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReimbursementTrackingApplication.Exceptions;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Models.DTOs;

using ReimbursementTrackingApplication.Services;

namespace ReimbursementTrackingApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class ReimbursementRequestController : ControllerBase
    {

        private readonly IReimbursementRequestService _reimbursementRequestService;
        public ReimbursementRequestController(IReimbursementRequestService reimbursementRequestService)
        {
            _reimbursementRequestService = reimbursementRequestService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<ActionResult<SuccessResponseDTO<ResponseReimbursementRequestDTO>>> AddRequestAsync([FromForm] CreateReimbursementRequestDTO request)
        {
            try
            {
                var result = await _reimbursementRequestService.SubmitRequestAsync(request);
                return Ok(result);

            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new ErrorResponseDTO()
                {
                    ErrorMessage = ex.Message,
                    ErrorNumber = StatusCodes.Status401Unauthorized
                }
                );
            }
            catch (CollectionEmptyException ex)
            {
                return Unauthorized(new ErrorResponseDTO()
                {
                    ErrorMessage = ex.Message,
                    ErrorNumber = StatusCodes.Status401Unauthorized
                }
                );

            }
            catch (Exception ex)
            {
                return Unauthorized(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<SuccessResponseDTO<ResponseReimbursementRequestDTO>>> GetRequestsById(int id)
        {
            try
            {
                var result = await _reimbursementRequestService.GetRequestByIdAsync(id);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }
        [HttpGet("all")]
        [Authorize]
        public async Task<ActionResult<SuccessResponseDTO<ResponseReimbursementRequestDTO>>> GetAllRequest(int pageNumber, int pageSize)
        {
            try
            {
                var result = await _reimbursementRequestService.GetAllRequest(pageNumber, pageSize);

                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<ActionResult<PaginatedResultDTO<ResponseReimbursementRequestDTO>>> GetRequestsByUserId(int userId, int pageNumber, int pageSize)
        {
            try
            {
                var result = await _reimbursementRequestService.GetRequestsByUserIdAsync(userId, pageNumber, pageSize);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }
        //     [HttpGet("status/{statusId}")]
        //     [Authorize]
        //     public async Task<ActionResult<PaginatedResultDTO<ResponseReimbursementRequestDTO>>> GetRequestsByStatusAsync(RequestStatus statusId, int pageNumber, int pageSize)
        //     {
        //         try
        //         {
        //             var result = await _reimbursementRequestService.GetRequestsByStatusAsync(statusId,pageNumber,pageSize);
        //             return Ok(result);

        //         }
        //         catch (Exception ex)
        //         {
        //             return BadRequest(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

        //         }
        //     }



        //    [HttpGet("policy/{policyId}")]
        //            [Authorize]
        //     public async Task<ActionResult<PaginatedResultDTO<ResponseReimbursementRequestDTO>>> GetRequestsByPolicy(int policyId, int pageNumber, int pageSize)
        //     {
        //         try
        //         {
        //             var result = await _reimbursementRequestService.GetRequestsByPolicyAsync(policyId, pageNumber, pageSize);
        //             return Ok(result);

        //         }
        //         catch (Exception ex)
        //         {
        //             return BadRequest(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

        //         }
        //     }
        //DeleteRequestAsync
        [HttpGet("manager/{managerId}")]
        public async Task<ActionResult<PaginatedResultDTO<ResponseReimbursementRequestDTO>>> GetRequestsByManager(int managerId, int pageNumber, int pageSize)
        {
            try
            {
                var result = await _reimbursementRequestService.GetRequestsByManagerIdAsync(managerId, pageNumber, pageSize);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<SuccessResponseDTO<int>>> DeleteRequestsById(int id)
        {
            try
            {
                var result = await _reimbursementRequestService.DeleteRequestAsync(id);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardResponseDTO>> GetDashboardData()
        {
            try
            {
                var result = await _reimbursementRequestService.GetDashboard();
                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });
            }
        }
    }
}
