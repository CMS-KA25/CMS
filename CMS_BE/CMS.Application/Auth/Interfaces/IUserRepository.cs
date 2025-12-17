using CMS.Domain.Auth.Entities;
using CMS.Domain.Auth.Enums;

namespace CMS.Application.Auth.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid userId);
        Task<User?> GetByEmailAsync(string email);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> ExistsByEmailAsync(string email);
        Task<IEnumerable<User>> GetUsersByRoleAsync(RoleType role);
    }
}

