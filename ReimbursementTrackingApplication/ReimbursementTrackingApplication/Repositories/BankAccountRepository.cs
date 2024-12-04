using Microsoft.EntityFrameworkCore;
using ReimbursementTrackingApplication.Context;
using ReimbursementTrackingApplication.Exceptions;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models;

namespace ReimbursementTrackingApplication.Repositories
{
    public class BankAccountRepository : IRepository<int, BankAccount>
    {
        public readonly ContextApp _context;
        private readonly ILogger<BankAccountRepository> _logger;

        public BankAccountRepository(ContextApp context, ILogger<BankAccountRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<BankAccount> Add(BankAccount entity)
        {
            try
            {
                _context.BankAccounts.Add(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not add bank");
                throw new CouldNotAddException("bank");
            }
        }

        public async Task<BankAccount> Delete(int key)
        {
            try
            {
                var bank = await Get(key);
                _context.BankAccounts.Remove(bank);
                await _context.SaveChangesAsync();
                return bank;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not delete bank");
                throw new NotFoundException("Unable to bank");
            }
        }

        public async Task<BankAccount> Get(int key)
        {
            try
            {
                var bank = await _context.BankAccounts.FirstOrDefaultAsync(u => u.Id == key);
                if (bank == null)
                {
                    throw new Exception();
                }
                return bank;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not get bank");
                throw new NotFoundException("bank");
            }
        }

        public async Task<IEnumerable<BankAccount>> GetAll()
        {
            var approvals = await _context.BankAccounts.ToListAsync();
            if (approvals.Count == 0)
            {
                throw new CollectionEmptyException("bank");
            }
            return approvals;
        }

        public async Task<BankAccount> Update(int key, BankAccount entity)
        {
            try
            {
                // Retrieve the entity from the database without tracking
                var existingBank = await _context.BankAccounts.AsNoTracking()
                    .FirstOrDefaultAsync(b => b.Id == key);

                if (existingBank == null)
                {
                    throw new Exception("Bank account not found");
                }

                // Check for already tracked entities and detach if necessary
                var trackedEntity = _context.ChangeTracker.Entries<BankAccount>()
                    .FirstOrDefault(e => e.Entity.Id == key);

                if (trackedEntity != null)
                {
                    _context.Entry(trackedEntity.Entity).State = EntityState.Detached;
                }

                // Update properties manually
                existingBank.UserId = entity.UserId;
                existingBank.AccNo = entity.AccNo;
                existingBank.BranchName = entity.BranchName;
                existingBank.IFSCCode = entity.IFSCCode;
                existingBank.BranchAddress = entity.BranchAddress;

                // Attach the modified entity
                _context.BankAccounts.Update(existingBank);

                // Save changes
                await _context.SaveChangesAsync();

                return existingBank;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not update bank details");
                throw new Exception("Unable to modify bank object", e);
            }
        }

    }
}
