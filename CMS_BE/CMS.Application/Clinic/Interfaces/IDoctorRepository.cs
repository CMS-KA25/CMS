using CMS.Application.Clinic.DTOs.Responses;
using CMS.Domain.Clinic.Entities;

namespace CMS.Application.Clinic.Interfaces
{
    public interface IDoctorRepository
    {
        Task<Doctor?> GetDoctorByIdAsync(Guid doctorId);
        Task<bool> IsDoctorActiveAsync(Guid doctorId);
        Task<List<Doctor>> GetAllActiveDoctorsAsync();
        Task<List<DoctorWithUserDto>> GetAllActiveDoctorsWithUserAsync();
    }
}