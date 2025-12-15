using CMS.Domain.Auth.Entities;
using CMS.Domain.Clinic.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.Clinic.Entities
{
    public class Doctor
    {
        public Guid DoctorID { get; set; }
        [Required]
        public string Specialization { get; set; }
        [Required]
        public string Qualification { get; set; }
        [Required]
        public int YearOfExperience { get; set; }
        [Required]
        public WorkingDays[] WorkingDays { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int SlotDuration { get; set; }
        public TimeSpan? BreakStartTime { get; set; }
        public TimeSpan? BreakEndTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
