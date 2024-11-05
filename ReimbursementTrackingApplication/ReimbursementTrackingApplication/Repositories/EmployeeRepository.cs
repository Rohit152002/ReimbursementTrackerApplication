using Microsoft.EntityFrameworkCore;
using ReimbursementTrackingApplication.Context;
using ReimbursementTrackingApplication.Exceptions;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models;

namespace ReimbursementTrackingApplication.Repositories
{
    public class EmployeeRepository : IRepository<int, Employee>
    {
        public readonly ContextApp _context;
        private readonly ILogger<EmployeeRepository> _logger;

        public EmployeeRepository(ContextApp context, ILogger<EmployeeRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<Employee> Add(Employee entity)
        {
            try
            {
                if (entity.ManagerId == 0)
                {
                    throw new CouldNotAddException("ManagerId is required for Employee");
                }
                _context.Employees.Add(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not add Employees");
                throw new CouldNotAddException(e.Message);
            }
        }

        public async Task<Employee> Delete(int key)
        {
            try
            {
                var employee = await Get(key);
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
                return employee;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not delete employees");
                throw new NotFoundException("Unable to delete");
            }
        }

        public async Task<Employee> Get(int key)
        {
            try
            {
                var employee = await _context.Employees.FirstOrDefaultAsync(u => u.Id == key);
                if (employee == null)
                {
                    throw new Exception();
                }
                return employee;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not get employee");
                throw new NotFoundException("Employee");
            }
        }

        public async Task<IEnumerable<Employee>> GetAll()
        {
            var employees = await _context.Employees.ToListAsync();
            if (employees.Count == 0)
            {
                throw new CollectionEmptyException("Request");
            }
            return employees;
        }

        public async Task<Employee> Update(int key, Employee entity)
        {
            try
            {
                var employee = await Get(key);
                employee.ManagerId = entity.ManagerId;
                employee.Update();
                _context.Employees.Update(employee);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not update employee details");
                throw new Exception("Unable to modify employee object");
            }
        }
    }
}
