using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models.DTOs;

namespace ReimbursementTrackingApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController:ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        [HttpPost]
        public async Task<ActionResult<SuccessResponseDTO<int>>> AssignEmployeeManager(EmployeeDTO employeeDTO)
        {
            try
            {
            var result = await _employeeService.AddEmployeeAsync(employeeDTO);
                return Ok(result);

            }catch(Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }

        [HttpDelete]
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
        public async Task<ActionResult<SuccessResponseDTO<ResponseEmployeeDTO>>> GetEmployeeById(int employeeId)
        {
            try
            {

            var result= await _employeeService.GetEmployeeByIdAsync(employeeId);
            return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }

        [HttpGet("manager/{managerId}")]
        public async Task<ActionResult<PaginatedResultDTO<ResponseEmployeeDTO>>> GetEmployeeByManagerId(int managerId,
                    [FromQuery] int pageNumber = 1,
                    [FromQuery] int pageSize = 10)
        {
            try
            {

            var result = await _employeeService.GetEmployeesByManagerIdAsync(managerId,pageNumber,pageSize);
            return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }

        [HttpPut("{employeeId}")]
        public async Task<ActionResult<SuccessResponseDTO<ResponseEmployeeDTO>>> UpdateEmployeeById(int employeeId,EmployeeDTO employeeDTO)
        {
            try
            {

            var result = await _employeeService.UpdateEmployeeAsync(employeeId,employeeDTO);
            return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { ErrorMessage = ex.Message, ErrorNumber = 404 });

            }
        }



    }
}
