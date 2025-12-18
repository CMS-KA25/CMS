using CMS.Data;
using CMS.Domain.Clinic.Entities;
using CMS.Domain.Clinic.Enums;
using CMS.Application.Clinic.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CMS.Infrastructure.Clinic.Repositories
{
    public class LeaveRepository : ILeaveRepository
    {
        private readonly CmsDbContext _context;

        public LeaveRepository(CmsDbContext context)
        {
            _context = context;
        }

        public async Task<List<Leave>> GetApprovedLeavesForDateAsync(Guid doctorId, DateTime date)
        {
            return await _context.Leaves
                .Where(l => l.UserID == doctorId 
                         && l.Status == LeaveStatus.Approved
                         && l.StartDate.Date <= date.Date 
                         && l.EndDate.Date >= date.Date)
                .ToListAsync();
        }
    }
}