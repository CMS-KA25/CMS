namespace CMS.Application.Appointments.DTOs.Responses
{
    public class TimeSlotDto
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string DisplayTime { get; set; } = string.Empty;
    }
}