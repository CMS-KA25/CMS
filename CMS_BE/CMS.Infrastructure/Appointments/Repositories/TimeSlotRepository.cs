using CMS.Data;
using CMS.Domain.Appointments.Entities;
using CMS.Application.Appointments.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CMS.Infrastructure.Appointments.Repositories
{
    public class TimeSlotRepository : ITimeSlotRepository
    {
        private readonly CmsDbContext _context;

        public TimeSlotRepository(CmsDbContext context)
        {
            _context = context;
        }

        public async Task<List<TimeSlot>> GetBookedSlotsAsync(Guid doctorId, DateTime date)
        {
            return await _context.TimeSlots
                .Where(ts => ts.DoctorID == doctorId 
                          && ts.SlotDate.Date == date.Date 
                          && !ts.IsAvailable)
                .ToListAsync();
        }

        public async Task<TimeSlot> CreateTimeSlotAsync(TimeSlot slot)
        {
            _context.TimeSlots.Add(slot);
            await _context.SaveChangesAsync();
            return slot;
        }
    }
}