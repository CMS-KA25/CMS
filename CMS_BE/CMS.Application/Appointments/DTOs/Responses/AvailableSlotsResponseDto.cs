namespace CMS.Application.Appointments.DTOs.Responses
{
    public class AvailableSlotsResponseDto
    {
        public DateTime Date { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public List<TimeSlotDto> AvailableSlots { get; set; } = new();
        public int TotalSlots { get; set; }
    }
}