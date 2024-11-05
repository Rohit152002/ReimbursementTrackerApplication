using Microsoft.EntityFrameworkCore;
using ReimbursementTrackingApplication.Context;
using ReimbursementTrackingApplication.Exceptions;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models;

namespace ReimbursementTrackingApplication.Repositories
{
    public class PaymentRepository : IRepository<int, Payment>
    {
        public readonly ContextApp _context;
        private readonly ILogger<PaymentRepository> _logger;

        public PaymentRepository(ContextApp context, ILogger<PaymentRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Payment> Add(Payment entity)
        {
            try
            {
                if (entity.RequestId == 0)
                {
                    throw new CouldNotAddException("RequestId is required for Paymemnt");
                }
                _context.Payments.Add(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not add payment");
                throw new CouldNotAddException(e.Message);
            }
        }

        public async Task<Payment> Delete(int key)
        {
            try
            {
                var payment = await Get(key);
                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();
                return payment;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not delete payment");
                throw new NotFoundException("Unable to delete");
            }
        }

        public async Task<Payment> Get(int key)
        {
            try
            {
                var payment = await _context.Payments.FirstOrDefaultAsync(u => u.Id == key);
                if (payment == null)
                {
                    throw new Exception();
                }
                return payment;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not get payment");
                throw new NotFoundException("payment");
            }
        }

        public async Task<IEnumerable<Payment>> GetAll()
        {
            var payment = await _context.Payments.ToListAsync();
            if (payment.Count == 0)
            {
                throw new CollectionEmptyException("Payment");
            }
            return payment;
        }

        public async Task<Payment> Update(int key, Payment entity)
        {
            try
            {
                var payment = await Get(key);
                _context.Payments.Update(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not update payment details");
                throw new Exception("Unable to modify payment object");
            }
        }
    }
}
