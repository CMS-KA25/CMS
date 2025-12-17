using CMS.Data;
using CMS.Domain.Auth.Entities;
using CMS.Domain.Auth.Enums;
using CMS.Application.Auth.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CMS.Infrastructure.Auth.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly CmsDbContext _context;

        public UserRepository(CmsDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserID == userId);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<User> CreateAsync(User user)
        {
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(RoleType role)
        {
            return await _context.Users
                .Where(u => u.Role == role)
                .ToListAsync();
        }
    }
}

