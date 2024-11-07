using ReimbursementTrackingApplication.Models.DTOs;

namespace ReimbursementTrackingApplication.Interfaces
{
    public interface IEmployeeService
    {
      
            Task<SuccessResponseDTO<ResponseEmployeeDTO>> GetEmployeeByIdAsync(int employeeId);
            Task<PaginatedResultDTO<ResponseEmployeeDTO>> GetEmployeesByManagerIdAsync(int managerId, int pageNumber, int pageSize);
            Task<PaginatedResultDTO<ResponseEmployeeDTO>> GetAllEmployeesAsync(int pageNumber, int pageSize);
            Task<SuccessResponseDTO<int>> AddEmployeeAsync(EmployeeDTO employeeDto);
            Task<SuccessResponseDTO<int>> UpdateEmployeeAsync(int employeeId, EmployeeDTO employeeDto);
            Task<SuccessResponseDTO<int>> DeleteEmployeeAsync(int employeeId);
        

    }
}
