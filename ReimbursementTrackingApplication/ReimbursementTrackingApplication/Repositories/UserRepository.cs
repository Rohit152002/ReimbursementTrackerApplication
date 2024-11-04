using Microsoft.EntityFrameworkCore;
using ReimbursementTrackingApplication.Context;
using ReimbursementTrackingApplication.Exceptions;
using ReimbursementTrackingApplication.Interfaces;
using ReimbursementTrackingApplication.Models;

namespace ReimbursementTrackingApplication.Repositories
{
    public class UserRepository : IRepository<int, User>
    {
        public readonly ContextApp _context;
        private readonly ILogger<UserRepository> _logger;
        public UserRepository(ContextApp context,ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<User> Add(User entity)
        {
            try
            {
                _context.Users.Add(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not add user");
                throw new CouldNotAddException("User");
            }
        }

        public async Task<User> Delete(int key)
        {
            try
            {
            var user = await Get(key);
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not delete user");
                throw new NotFoundException("Unable to delete");
            }
        }

        public async Task<User> Get(int key)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == key);
                if (user==null)
                {
                    throw new Exception();
                }
                return user;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not get user");
                throw new NotFoundException("User");
            }
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            var users = await _context.Users.ToListAsync();
            if (users.Count == 0)
            {
                throw new CollectionEmptyException("Users");
            }
            return users;
        }

        public async Task<User> Update(int key, User entity)
        {
            try
            {
            var user = await Get(key);
                _context.Users.Update(entity);
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
