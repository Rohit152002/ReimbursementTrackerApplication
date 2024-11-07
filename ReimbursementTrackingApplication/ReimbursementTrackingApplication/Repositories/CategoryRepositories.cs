using Microsoft.EntityFrameworkCore;
using ReimbursementTrackingApplication.Context;
using ReimbursementTrackingApplication.Exceptions;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models;

namespace ReimbursementTrackingApplication.Repositories
{
    public class CategoryRepositories : IRepository<int, ExpenseCategory>
    {
        public readonly ContextApp _context;
        private readonly ILogger<CategoryRepositories> _logger;

        public CategoryRepositories(ContextApp context, ILogger<CategoryRepositories> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<ExpenseCategory> Add(ExpenseCategory entity)
        {
            try
            {
                _context.Expenses.Add(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not add Expense");
                throw new CouldNotAddException("Expense");
            }
        }

        public async Task<ExpenseCategory> Delete(int key)
        {
            try
            {
                var request = await Get(key);
                _context.Expenses.Remove(request);
                await _context.SaveChangesAsync();
                return request;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not delete Expenses");
                throw new NotFoundException("Unable to delete");
            }
        }

        public async Task<ExpenseCategory> Get(int key)
        {
            try
            {
                var request = await _context.Expenses.FirstOrDefaultAsync(u => u.Id == key);
                if (request == null)
                {
                    throw new Exception("Expenses not found");
                }
                return request;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not get expenses");
                throw new NotFoundException("Expenses");
            }
        }

        public async Task<IEnumerable<ExpenseCategory>> GetAll()
        {
            var request = await _context.Expenses.ToListAsync();
            if (request.Count == 0)
            {
                throw new CollectionEmptyException("Request");
            }
            return request;
        }

        public async Task<ExpenseCategory> Update(int key, ExpenseCategory entity)
        {
            try
            {
                var category = await Get(key);
                _context.Expenses.Update(entity);
              
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not update user details");
                throw new Exception("Unable to modify user object");
            }
        }
    }
}
