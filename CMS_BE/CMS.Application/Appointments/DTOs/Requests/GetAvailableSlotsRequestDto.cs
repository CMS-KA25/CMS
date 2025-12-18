using CMS.Domain.Auth.Enums;
using System.ComponentModel.DataAnnotations;

namespace CMS.Application.Appointments.DTOs.Requests
{
    public class GetAvailableSlotsRequestDto
    {
        [Required]
        public Guid DoctorId { get; set; }
        
        [Required]
        public DateTime Date { get; set; }
        
        [Required]
        public RoleType UserRole { get; set; }
    }
}