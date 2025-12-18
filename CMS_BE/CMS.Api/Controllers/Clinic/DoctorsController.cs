using CMS.Application.Clinic.DTOs.Responses;
using CMS.Application.Clinic.Interfaces;
using CMS.Application.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Api.Controllers.Clinic
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly ILogger<DoctorsController> _logger;

        public DoctorsController(IDoctorService doctorService, ILogger<DoctorsController> logger)
        {
            _doctorService = doctorService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<DoctorDto>>>> GetAllActiveDoctors()
        {
            try
            {
                var doctors = await _doctorService.GetAllActiveDoctorsAsync();
                return Ok(ApiResponse<List<DoctorDto>>.SuccessResponse(doctors, $"Found {doctors.Count} active doctors"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching active doctors");
                return StatusCode(500, ApiResponse<List<DoctorDto>>.ErrorResponse("An error occurred while fetching doctors"));
            }
        }
    }
}
