using Microsoft.EntityFrameworkCore;
using ReimbursementTrackingApplication.Context;
using ReimbursementTrackingApplication.Exceptions;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models;

namespace ReimbursementTrackingApplication.Repositories
{
    public class ApprovalStageRepository : IRepository<int, ApprovalStage>
    {
        public readonly ContextApp _context;
        private readonly ILogger<ApprovalStageRepository> _logger;

        public ApprovalStageRepository(ContextApp context, ILogger<ApprovalStageRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<ApprovalStage> Add(ApprovalStage entity)
        {
            try
            {
                _context.Approvals.Add(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not add Approvals");
                throw new CouldNotAddException("Approvals");
            }
        }

        public async Task<ApprovalStage> Delete(int key)
        {
            try
            {
                var approval = await Get(key);
                _context.Approvals.Remove(approval);
                await _context.SaveChangesAsync();
                return approval;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not delete Approval");
                throw new NotFoundException("Unable to approval");
            }
        }

        public async Task<ApprovalStage> Get(int key)
        {
            try
            {
                var request = await _context.Approvals.FirstOrDefaultAsync(u => u.Id == key);
                if (request == null)
                {
                    throw new Exception();
                }
                return request;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not get approvals");
                throw new NotFoundException("Approvals");
            }
        }

        public async Task<IEnumerable<ApprovalStage>> GetAll()
        {
            var approvals = await _context.Approvals.ToListAsync();
            if (approvals.Count == 0)
            {
                //throw new CollectionEmptyException("Approvals");
                return null;
            }
            return approvals;
        }

        public async Task<ApprovalStage> Update(int key, ApprovalStage entity)
        {
            try
            {
                var user = await Get(key);
                _context.Approvals.Update(entity);
                
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not update approvals details");
                throw new Exception("Unable to modify approvals object");
            }
        }
    }
}
