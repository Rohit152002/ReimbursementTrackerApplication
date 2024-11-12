using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models.DTOs;

namespace ReimbursementTrackingApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PolicyController : ControllerBase
    {
        private readonly IPolicyService _policyService;
        public PolicyController(IPolicyService policyService)
        {
            _policyService = policyService;
        }
        //Task<SuccessResponseDTO<ResponsePolicyDTO>> GetPolicyByIdAsync(int policyId);
        [HttpGet("{id}")]
        public async Task<ActionResult<SuccessResponseDTO<ResponsePolicyDTO>>> GetPolicyByid(int id)
        {
            try
            {

                var policy = await _policyService.GetPolicyByIdAsync(id);
                return Ok(policy);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }


        }
        //Task<PaginatedResultDTO<ResponsePolicyDTO>> GetAllPolicesAsync(int pageNumber, int pageSize);
        [HttpGet]
        public async Task<ActionResult<PaginatedResultDTO<ResponsePolicyDTO>>> GetAllPolicies(int pageNumber=1, int pageSize =10)
        {
            try
            {

                var policies = await _policyService.GetAllPolicesAsync(pageNumber,pageSize);
                return Ok(policies);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }


        }
        //Task<SuccessResponseDTO<int>> AddPolicyAsync(CreatePolicyDTO policyDTO);
        [HttpPost]
        public async Task<ActionResult<SuccessResponseDTO<int>>> AddPolicies(CreatePolicyDTO policyDTO)
        {
            try
            {

                var policies = await _policyService.AddPolicyAsync(policyDTO);
                return Ok(policies);
            }
            catch (Exception ex)
            {
                return Ok(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }
        //Task<SuccessResponseDTO<int>> UpdatePolicyAsync(int categoryId, CreatePolicyDTO policyDTO);
        [HttpPut("{id}")]
        public async Task<ActionResult<SuccessResponseDTO<int>>> UpdatePolicy(int id, CreatePolicyDTO policyDTO)
        {
            try
            {

                var policies = await _policyService.UpdatePolicyAsync(id,policyDTO);
                return Ok(policies);
            }
            catch (Exception ex)
            {
                return Ok(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }

        //Task<SuccessResponseDTO<int>> DeletePolicyAsync(int policyId);

        [HttpDelete("{id}")]
        public async Task<ActionResult<SuccessResponseDTO<int>>> Delete(int id)
        {
            try
            {

                var policies = await _policyService.DeletePolicyAsync(id);
                return Ok(policies);
            }
            catch (Exception ex)
            {
                return Ok(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }

    }
}
