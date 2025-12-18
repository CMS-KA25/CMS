using CMS.Application.Appointments.DTOs.Requests;
using CMS.Application.Appointments.DTOs.Responses;
using CMS.Domain.Auth.Enums;

namespace CMS.Application.Appointments.Interfaces
{
    public interface ITimeSlotService
    {
        Task<AvailableSlotsResponseDto> GetAvailableSlotsAsync(GetAvailableSlotsRequestDto request);
        Task<bool> ValidateBookingWindowAsync(DateTime date, RoleType role);
    }
}