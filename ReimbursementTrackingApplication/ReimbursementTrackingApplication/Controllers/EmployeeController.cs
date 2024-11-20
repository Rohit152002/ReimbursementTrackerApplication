using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

namespace ReimbursementTrackingApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SuccessResponseDTO<int>>> AssignEmployeeManager(EmployeeDTO employeeDTO)
        {
            try
            {
                var result = await _employeeService.AddEmployeeAsync(employeeDTO);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }

        [HttpDelete("{employeeId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SuccessResponseDTO<int>>> DeleteAssignEmployee(int employeeId)

        {
            try
            {

                var result = await _employeeService.DeleteEmployeeAsync(employeeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }

        [HttpGet("employees")]
        [Authorize]
        public async Task<ActionResult<SuccessResponseDTO<ResponseEmployeeDTO>>> GetAllEmployee(int pageNumber, int pageSize)
        {
            try
            {

                var result = await _employeeService.GetAllEmployeesAsync(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }

        [HttpGet("{employeeId}")]
        [Authorize]
        public async Task<ActionResult<SuccessResponseDTO<ResponseEmployeeDTO>>> GetEmployeeById(int employeeId)
        {
            try
            {

                var result = await _employeeService.GetEmployeeByIdAsync(employeeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }

        [HttpGet("manager/{managerId}")]
        [Authorize]
        public async Task<ActionResult<PaginatedResultDTO<ResponseEmployeeDTO>>> GetEmployeeByManagerId(int managerId,
                    [FromQuery] int pageNumber = 1,
                    [FromQuery] int pageSize = 10)
        {
            try
            {

                var result = await _employeeService.GetEmployeesByManagerIdAsync(managerId, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }

        [HttpPut("{employeeId}")]
        [Authorize]
        public async Task<ActionResult<SuccessResponseDTO<ResponseEmployeeDTO>>> UpdateEmployeeById(int employeeId, EmployeeDTO employeeDTO)
        {
            try
            {

                var result = await _employeeService.UpdateEmployeeAsync(employeeId, employeeDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }

        [HttpGet("withoutmanager")]
        public async Task<ActionResult> GetUserWithNoManager(int pageNumber, int pageSize)
        {
            try
            {
                var result = await _employeeService.GetUsersWithoutAssignedManagerAsync(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO()
                {
                    ErrorMessage = ex.Message,
                    ErrorNumber = StatusCodes.Status400BadRequest
                });
            }

        }



    }
}
