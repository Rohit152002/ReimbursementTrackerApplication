using AutoMapper;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Models.DTOs;
using ReimbursementTrackingApplication.Repositories;
using System.Runtime.CompilerServices;

namespace ReimbursementTrackingApplication.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepository<int,Employee> _repository;
        private readonly UserService _userService;
        private readonly IMapper _mapper;
        public EmployeeService(IRepository<int,Employee> repository,IMapper mapper, UserService userService)
        {
            _repository = repository;
            _mapper = mapper;
            _userService = userService;
        }
        public async Task<SuccessResponseDTO<int>> AddEmployeeAsync(EmployeeDTO employeeDto)
        {
            try
            {
                var employee = _mapper.Map<Employee>(employeeDto);
                var addedEmployee= await  _repository.Add(employee);

                return new SuccessResponseDTO<int>()
                {
                    IsSuccess = true,
                    Message="Employee Added Successfully",
                    Data=addedEmployee.EmployeeId
                };


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<SuccessResponseDTO<int>> DeleteEmployeeAsync(int employeeId)
        {
            try
            {
                var employee = await _repository.Get(employeeId); 
                employee.IsDeleted = true;
                var updateEmployee = await _repository.Update(employeeId, employee);
                return new SuccessResponseDTO<int>() { 
                IsSuccess=true,
                Message="Employee Deleted Successfully",
                Data=updateEmployee.EmployeeId};
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<PaginatedResultDTO<ResponseEmployeeDTO>> GetAllEmployeesAsync(int pageNumber, int pageSize)
        {
            try
            {
                var employees = (await _repository.GetAll()).Where(r=>r.IsDeleted==false);
          

                List<ResponseEmployeeDTO> result = new List<ResponseEmployeeDTO>();
                foreach(var employee in employees)
                {
                    var newEmployeeDTO = new ResponseEmployeeDTO()
                    {
                        EmployeeId = employee.EmployeeId,
                        Employee= await _userService.GetUserProfile(employee.EmployeeId),
                        ManagerId = employee.ManagerId,
                        Manager = await _userService.GetUserProfile(employee.ManagerId),
                    };
                    result.Add(newEmployeeDTO);

                }
                var total = result.Count();
                var pagedItems = result
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return new PaginatedResultDTO<ResponseEmployeeDTO>
                {
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalCount = total,
                    TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                    Data = pagedItems
                };

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async  Task<SuccessResponseDTO<ResponseEmployeeDTO>> GetEmployeeByIdAsync(int employeeId)
        {
            try
            {
                var employee = await _repository.Get(employeeId);
                ResponseEmployeeDTO result = new ResponseEmployeeDTO()
                {
                    EmployeeId = employee.EmployeeId,
                    Employee = await _userService.GetUserProfile(employee.EmployeeId),
                    ManagerId= employee.ManagerId,
                    Manager = await _userService.GetUserProfile(employee.ManagerId)
                };

                return new SuccessResponseDTO<ResponseEmployeeDTO> {
                    IsSuccess = true,
                    Message="Fetch Succesfully",
                    Data = result };
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not find {employeeId}");
            }
        }

        public async Task<PaginatedResultDTO<ResponseEmployeeDTO>> GetEmployeesByManagerIdAsync(int managerId, int pageNumber, int pageSize)
        {
            try
            {
                var employess = (await _repository.GetAll()).Where(e=>e.Id==managerId).ToList();
                List<ResponseEmployeeDTO> result = new List<ResponseEmployeeDTO>();
                foreach (var employee in employess)
                {
                    var newEmployeeDTO = new ResponseEmployeeDTO()
                    {
                        EmployeeId = employee.EmployeeId,
                        Employee = await _userService.GetUserProfile(employee.EmployeeId),
                        ManagerId = employee.ManagerId,
                        Manager = await _userService.GetUserProfile(employee.ManagerId),
                    };
                    result.Add(newEmployeeDTO);

                }
                var total = result.Count();
                var pagedItems = result
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return new PaginatedResultDTO<ResponseEmployeeDTO>
                {
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalCount = total,
                    TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                    Data = pagedItems
                };

            }
            catch (Exception ex)
            {
                throw new Exception (ex.Message);
            }
        }

        public async Task<SuccessResponseDTO<int>> UpdateEmployeeAsync(int employeeId, EmployeeDTO employeeDto)
        {
            try
            {
                var employee = _mapper.Map<Employee>(employeeDto);
                var updateEmployee= await _repository.Update(employeeId, employee);
                return new SuccessResponseDTO<int> { 
                    IsSuccess = true,
                    Message="Successfully updated",
                    Data=updateEmployee.Id
                };
            }
            catch (Exception ex)
            { throw new Exception (ex.Message); }

        }
    }
}
