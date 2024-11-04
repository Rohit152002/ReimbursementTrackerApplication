using Microsoft.EntityFrameworkCore;
using ReimbursementTrackingApplication.Context;
using ReimbursementTrackingApplication.Exceptions;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models;

namespace ReimbursementTrackingApplication.Repositories
{
    public class ReimbursementRequestRepository : IRepository<int, ReimbursementRequest>
    {
        private readonly ContextApp _context;
        private readonly ILogger<ReimbursementRequestRepository> _logger;

        public ReimbursementRequestRepository(ContextApp context, ILogger<ReimbursementRequestRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ReimbursementRequest> Add(ReimbursementRequest entity)
        {
            try
            {
                _context.ReimbursementRequests.Add(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not add request");
                throw new CouldNotAddException("Request");
            }
        }

        public async Task<ReimbursementRequest> Delete(int key)
        { 
            try
            {
                var request = await Get(key);
                _context.ReimbursementRequests.Remove(request);
                await _context.SaveChangesAsync();
                return request;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not delete Request");
                throw new NotFoundException("Unable to delete");
            }
        }

        public async Task<ReimbursementRequest> Get(int key)
        {
            try
            {
                var request = await _context.ReimbursementRequests.FirstOrDefaultAsync(u => u.Id == key);
                if (request == null)
                {
                    throw new Exception();
                }
                return request;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not get Request");
                throw new NotFoundException("Request");
            }
        }

        public async Task<IEnumerable<ReimbursementRequest>> GetAll()
        {
            var request = await _context.ReimbursementRequests.ToListAsync();
            if (request.Count == 0)
            {
                throw new CollectionEmptyException("Request");
            }
            return request;
        }

        public async Task<ReimbursementRequest> Update(int key, ReimbursementRequest entity)
        {
            try
            {
                var user = await Get(key);
                _context.ReimbursementRequests.Update(entity);
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
