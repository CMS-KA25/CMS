using CMS.Application.Clinic.DTOs.Responses;
using CMS.Application.Clinic.Interfaces;
using Microsoft.Extensions.Logging;

namespace CMS.Application.Clinic.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly ILogger<DoctorService> _logger;

        public DoctorService(
            IDoctorRepository doctorRepository,
            ILogger<DoctorService> logger)
        {
            _doctorRepository = doctorRepository;
            _logger = logger;
        }

        public async Task<List<DoctorDto>> GetAllActiveDoctorsAsync()
        {
            _logger.LogInformation("Fetching all active doctors");

            var doctors = await _doctorRepository.GetAllActiveDoctorsWithUserAsync();

            var doctorDtos = doctors.Select(d => new DoctorDto
            {
                Id = d.DoctorID,
                Name = d.Name,
                Specialization = d.Specialization,
                YearOfExperience = d.YearOfExperience,
                WorkingDays = d.WorkingDays
            }).ToList();

            _logger.LogInformation("Found {Count} active doctors", doctorDtos.Count);
            return doctorDtos;
        }
    }
}
