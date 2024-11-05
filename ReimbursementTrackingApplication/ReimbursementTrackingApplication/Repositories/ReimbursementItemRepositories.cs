using Microsoft.EntityFrameworkCore;
using ReimbursementTrackingApplication.Context;
using ReimbursementTrackingApplication.Exceptions;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models;

namespace ReimbursementTrackingApplication.Repositories
{
    public class ReimbursementItemRepositories : IRepository<int, ReimbursementItem>
    {
        private readonly ContextApp _context;
        private readonly ILogger<ReimbursementItemRepositories> _logger;
        public ReimbursementItemRepositories(ContextApp context, ILogger<ReimbursementItemRepositories> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<ReimbursementItem> Add(ReimbursementItem entity)
        {
            try
            {
                _context.ReimbursementItems.Add(entity);
                await _context.SaveChangesAsync();
                return  entity;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not add item");
                throw new CouldNotAddException("item");
            }
        }

        public async Task<ReimbursementItem> Delete(int key)
        {
            try
            {
                var item = await Get(key);
                _context.ReimbursementItems.Remove(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not delete Request");
                throw new NotFoundException("Unable to delete");
            }
        }

        public async Task<ReimbursementItem> Get(int key)
        {
            try
            {
                var item = await _context.ReimbursementItems.FirstOrDefaultAsync(u => u.Id == key);
                if (item == null)
                {
                    throw new Exception();
                }
                return  item;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not get Request");
                throw new NotFoundException("Request");
            }
        }

        public async Task<IEnumerable<ReimbursementItem>> GetAll()
        {
            var items = await _context.ReimbursementItems.ToListAsync();
            if (items.Count == 0)
            {
                throw new CollectionEmptyException("Request");
            }
            return items;
        }

        public async Task<ReimbursementItem> Update(int key, ReimbursementItem entity)
        {
            try
            {
                var item = await Get(key);
                _context.ReimbursementItems.Update(entity);
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
