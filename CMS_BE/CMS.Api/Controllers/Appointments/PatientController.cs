using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CMS.Application;


namespace CMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly CMS.Application.PatientService _patientService;
        public PatientController(CMS.Application.PatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPatients()
        {
            var patients = await _patientService.GetAllPatients();
            return Ok(patients);
        }

        [HttpPost]
        public async Task<IActionResult> AddPatient([FromBody] Domain.Patient patient)
        {
            try
            {
                var newPatient = await _patientService.CreatePatient(patient);
                return Ok(new { message = "Patient created successfully", patient = newPatient });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating patient", error = ex.Message });
            }
        }
    }
}
