﻿using Microsoft.AspNetCore.Authorization;
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
    public class BankController : ControllerBase
    {
        private readonly IBankService _bankService;
        public BankController(IBankService bankService)
        {
            _bankService = bankService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<SuccessResponseDTO<ResponseBankDTO>>> AddBankAccount(BankDTO bankAccount)
        {
            try
            {

                var bank = await _bankService.AddBankAccountAsync(bankAccount);
                return Ok(bank);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }


        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<SuccessResponseDTO<ResponseBankDTO>>> UpdateBankAccount(int id, BankDTO bankAccount)
        {
            try
            {

                var bank = await _bankService.UpdateBankAccountAsync(id,bankAccount);
                return Ok(bank);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }


        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<SuccessResponseDTO<int>>> DeleteBankAccount(int id)
        {
            try
            {

                var bank = await _bankService.DeleteBankAccountAsync(id);
                return Ok(bank);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }


        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<SuccessResponseDTO<ResponseBankDTO>>> GetBankAccountById(int id)
        {
            try
            {

                var bank = await _bankService.GetBankAccountByIdAsync(id);
                return Ok(bank);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }


        }

        [HttpGet("user/{id}")]
                [Authorize]
        public async Task<ActionResult<SuccessResponseDTO<ResponseBankDTO>>> GetBankAccountByUserId(int id)
        {
            try
            {

                var bank = await _bankService.GetBankAccountByUserIdAsync(id);
                return Ok(bank);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }


        }
        [HttpGet("all")]
        [Authorize]
        public async Task<ActionResult<PaginatedResultDTO<ResponseBankDTO>>> GetAllBankAccounts(int pageNumber, int pageSize)
        {
            try
            {

                var bank = await _bankService.GetAllBankAccountsAsync(pageNumber,pageSize);
                return Ok(bank);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }


        }
    }
}
