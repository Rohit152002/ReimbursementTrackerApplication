using Microsoft.EntityFrameworkCore;
using ReimbursementTrackingApplication.Context;
using ReimbursementTrackingApplication.Exceptions;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models;

namespace ReimbursementTrackingApplication.Repositories
{
    public class PolicyRepository : IRepository<int, Policy>
    {
        public readonly ContextApp _context;
        private readonly ILogger<PolicyRepository> _logger;

        public PolicyRepository(ContextApp context, ILogger<PolicyRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Policy> Add(Policy entity)
        {
            try
            {
                _context.Policy.Add(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not add policy");
                throw new CouldNotAddException("policy");
            }
        }

        public async Task<Policy> Delete(int key)
        {
            try
            {
                var bank = await Get(key);
                _context.Policy.Remove(bank);
                await _context.SaveChangesAsync();
                return bank;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not delete policy");
                throw new NotFoundException("Unable to policy");
            }
        }

        public async Task<Policy> Get(int key)
        {
            try
            {
                var policy = await _context.Policy.FirstOrDefaultAsync(u => u.Id == key);
                if (policy == null)
                {
                    throw new Exception();
                }
                return policy;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not get policy");
                throw new NotFoundException("policy");
            }
        }

        public async Task<IEnumerable<Policy>> GetAll()
        {
            var policies = await _context.Policy.ToListAsync();
            if (policies.Count == 0)
            {
                throw new CollectionEmptyException("policy");
            }
            return policies;
        }

        public async Task<Policy> Update(int key, Policy entity)
        {
            try
            {
                var policy = await Get(key);
                policy.PolicyName = entity.PolicyName;
                policy.MaxAmount = entity.MaxAmount;
                policy.PolicyDescription = entity.PolicyDescription;
                _context.Policy.Update(policy);
               
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not update policy details");
                throw new Exception("Unable to modify policy object");
            }
        }
    }
}
