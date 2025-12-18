using CMS.Application.Clinic.DTOs.Responses;

namespace CMS.Application.Clinic.Interfaces
{
    public interface IDoctorService
    {
        Task<List<DoctorDto>> GetAllActiveDoctorsAsync();
    }
}
