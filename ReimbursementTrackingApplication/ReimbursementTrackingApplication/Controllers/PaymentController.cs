﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReimbursementTrackingApplication.Models.DTOs;
using ReimbursementTrackingApplication.Services;

namespace ReimbursementTrackingApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        public PaymentController(PaymentService paymentService)
        {
            _paymentService=paymentService;

        }

        // GET: api/payment/{requestId}
        [HttpGet("{requestId}")]
        public async Task<ActionResult<SuccessResponseDTO<ResponsePayment>>> GetPaymentByRequestId(int requestId)
        {
            try
            {
                var payment = await _paymentService.GetPaymentByRequestIdAsync(requestId);
                return Ok(payment);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorResponseDTO { ErrorMessage = ex.Message, ErrorNumber = 404 });
            }
        }

        // GET: api/payment/user/{userId}?pageNumber=1&pageSize=10
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<PaginatedResultDTO<ResponsePayment>>> GetPaymentsByUserId(int userId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var payments = await _paymentService.GetPaymentsByUserIdAsync(userId, pageNumber, pageSize);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorResponseDTO { ErrorMessage = ex.Message, ErrorNumber = 404 });
            }
        }

        // GET: api/payment/all?pageNumber=1&pageSize=10
        [HttpGet("all")]
        public async Task<ActionResult<PaginatedResultDTO<ResponsePayment>>> GetAllPayments(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var payments = await _paymentService.GetAllPaymentsAsync(pageNumber, pageSize);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorResponseDTO { ErrorMessage = ex.Message, ErrorNumber = 404 });
            }
        }

        // POST: api/payment
        [HttpPost]
        public async Task<ActionResult<SuccessResponseDTO<ResponsePayment>>> ProcessPayment(PaymentDTO paymentDTO)
        {
            try
            {
                var result = await _paymentService.ProcessPaymentAsync(paymentDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO { ErrorMessage = ex.Message, ErrorNumber = 400 });
            }
        }

        // DELETE: api/payment/{paymentId}
        [HttpDelete("{paymentId}")]
        public async Task<ActionResult<SuccessResponseDTO<int>>> DeletePayment(int paymentId)
        {
            try
            {
                var result = await _paymentService.DeletePaymentAsync(paymentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorResponseDTO { ErrorMessage = ex.Message, ErrorNumber = 404 });
            }
        }
    }
}