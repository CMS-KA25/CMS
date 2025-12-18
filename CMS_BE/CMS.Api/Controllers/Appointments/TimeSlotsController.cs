using CMS.Application.Appointments.DTOs.Requests;
using CMS.Application.Appointments.DTOs.Responses;
using CMS.Application.Appointments.Interfaces;
using CMS.Application.Shared.DTOs;
using CMS.Domain.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Api.Controllers.Appointments
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimeSlotsController : ControllerBase
    {
        private readonly ITimeSlotService _timeSlotService;
        private readonly ILogger<TimeSlotsController> _logger;

        public TimeSlotsController(ITimeSlotService timeSlotService, ILogger<TimeSlotsController> logger)
        {
            _timeSlotService = timeSlotService;
            _logger = logger;
        }

        [HttpGet("available")]
        public async Task<ActionResult<ApiResponse<AvailableSlotsResponseDto>>> GetAvailableSlots(
            [FromQuery] GetAvailableSlotsRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<AvailableSlotsResponseDto>.ErrorResponse(
                        "Invalid request parameters"));
                }

                var result = await _timeSlotService.GetAvailableSlotsAsync(request);
                
                return Ok(ApiResponse<AvailableSlotsResponseDto>.SuccessResponse(
                    result, 
                    $"Found {result.TotalSlots} available slots"));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("Resource not found: {Message}", ex.Message);
                return NotFound(ApiResponse<AvailableSlotsResponseDto>.ErrorResponse(ex.Message));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning("Validation error: {Message}", ex.Message);
                return BadRequest(ApiResponse<AvailableSlotsResponseDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available slots for doctor {DoctorId} on {Date}", 
                    request.DoctorId, request.Date);
                return StatusCode(500, ApiResponse<AvailableSlotsResponseDto>.ErrorResponse(
                    "An error occurred while processing your request"));
            }
        }
    }
}