using CMS.Application.Auth.Interfaces;
using CMS.Data;
using CMS.Domain.Auth.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Infrastructure.Auth.Repositories
{
    public class InvitationRepository : IInvitationRepository
    {
        private readonly CmsDbContext _context;
        public InvitationRepository(CmsDbContext context)
        {
            _context = context;
        }

        public async Task<Invitation> CreateAsync(Invitation invitation)
        {
            _context.Invitations.Add(invitation);
            await _context.SaveChangesAsync();
            return invitation;
        }

        public async Task<IEnumerable<Invitation>> CreateBulkAsync(IEnumerable<Invitation> invitations)
        {
            _context.Invitations.AddRange(invitations);
            await _context.SaveChangesAsync();
            return invitations.ToList();
        }

        public async Task<Invitation?> GetByTokenAsync(string token)
        {
            return await _context.Invitations.FirstOrDefaultAsync(i => i.Token == token);
        }

        public async Task<Invitation?> GetLatestByEmailAsync(string email)
        {
            return await _context.Invitations
                .Where(i => i.Email == email)
                .OrderByDescending(i => i.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<Invitation> UpdateAsync(Invitation invitation)
        {
            _context.Invitations.Update(invitation);
            await _context.SaveChangesAsync();
            return invitation;
        }
    }
}
