namespace CMS.Application.Clinic.DTOs.Responses
{
    public class DoctorWithUserDto
    {
        public Guid DoctorID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public int YearOfExperience { get; set; }
    }
}
