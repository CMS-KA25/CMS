using CMS.Application.Auth.Interfaces;
using CMS.Data;
using CMS.Domain.Auth.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CMS.Infrastructure.Auth.Repositories
{
    public class VerificationCodeRepository : IVerificationCodeRepository
    {
        private readonly CmsDbContext _context;
        public VerificationCodeRepository(CmsDbContext context)
        {
            _context = context;
        }

        public async Task<VerificationCode> CreateAsync(VerificationCode code)
        {
            _context.VerificationCodes.Add(code);
            await _context.SaveChangesAsync();
            return code;
        }

        public async Task<VerificationCode?> GetLatestAsync(Guid userId, string purpose)
        {
            return await _context.VerificationCodes
                .Where(v => v.UserID == userId && v.Purpose == purpose)
                .OrderByDescending(v => v.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<VerificationCode> UpdateAsync(VerificationCode code)
        {
            _context.VerificationCodes.Update(code);
            await _context.SaveChangesAsync();
            return code;
        }
    }
}
