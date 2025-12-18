using CMS.Domain.Appointments.Entities;
using CMS.Domain.Clinic.Entities;

namespace CMS.Application.Shared.Configuration
{
    public class SlotGenerationContext
    {
        public Doctor Doctor { get; set; } = null!;
        public DateTime RequestedDate { get; set; }
        public List<Leave> ApprovedLeaves { get; set; } = new();
        public List<TimeSlot> BookedSlots { get; set; } = new();
        public List<TimeSlot> GeneratedSlots { get; set; } = new();
    }
}