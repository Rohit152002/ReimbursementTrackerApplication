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
        //private readonly IUserServices _userService;
        private readonly IRepository<int,User> _userRepository;
        private readonly IMapper _mapper;
        public EmployeeService(IRepository<int,Employee> repository,
            IMapper mapper,
            //IUserServices userService,
            IRepository<int,User> userRepository)
        {
            _repository = repository;
            _mapper = mapper;
            //_userService = userService;
            _userRepository = userRepository;
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
                    Data=addedEmployee.Id
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
                        Id = employee.Id,
                        EmployeeId = employee.EmployeeId,

                        Employee = await GetUser(employee.EmployeeId),
                        ManagerId = employee.ManagerId,
                        Manager = await GetUser(employee.EmployeeId)
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

        public async Task<UserDTO> GetUser(int id)
        {
            var user = await _userRepository.Get(id);
             var userDTO = _mapper.Map<UserDTO>(user);
            return userDTO;
           
        }

        public async  Task<SuccessResponseDTO<ResponseEmployeeDTO>> GetEmployeeByIdAsync(int employeeId)
        {
            try
            {
                var employee = await _repository.Get(employeeId);
                ResponseEmployeeDTO result = new ResponseEmployeeDTO()
                {
                    Id = employee.Id,
                    EmployeeId = employee.EmployeeId,
                    Employee = await GetUser(employee.EmployeeId),
                    ManagerId= employee.ManagerId,
                    Manager = await GetUser(employee.ManagerId)
                };

                return new SuccessResponseDTO<ResponseEmployeeDTO> {
                    IsSuccess = true,
                    Message="Fetch Succesfully",
                    Data = result };
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not find {employeeId} {ex.Message} ");
            }
        }

        public async Task<PaginatedResultDTO<ResponseEmployeeDTO>> GetEmployeesByManagerIdAsync(int managerId, int pageNumber=1, int pageSize=1000)
        {
            try
            {
                var employess = (await _repository.GetAll()).Where(e=>e.ManagerId==managerId).ToList();
               
                List<ResponseEmployeeDTO> result = new List<ResponseEmployeeDTO>();
                foreach (var employee in employess)
                {
                    var newEmployeeDTO = new ResponseEmployeeDTO()
                    {
                        Id = employee.Id,
                        EmployeeId = employee.EmployeeId,
                        Employee = await GetUser(employee.EmployeeId),
                        ManagerId = employee.ManagerId,
                        Manager = await GetUser(employee.ManagerId),
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
