using CMS.Domain.Appointments.Entities;

namespace CMS.Application.Appointments.Interfaces
{
    public interface ITimeSlotRepository
    {
        Task<List<TimeSlot>> GetBookedSlotsAsync(Guid doctorId, DateTime date);
        Task<TimeSlot> CreateTimeSlotAsync(TimeSlot slot);
    }
}