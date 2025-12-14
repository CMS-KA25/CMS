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
        [Required]
        public Guid DoctorID { get; set; }
        [Required]
        public string Specialization { get; set; }
        [Required]
        public string Qualification { get; set; }
        [Required]
        public int YearOfExperience { get; set; }
        [Required]
        public WorkingDays[] WorkingDays { get; set; }
        public TimeSpan StartTime { get; set; } = new TimeSpan(9, 0, 0);
        public TimeSpan EndTime { get; set; } = new TimeSpan(18, 0, 0);
        public int SlotDuration { get; set; } = 30;
        public TimeSpan? BreakStartTime { get; set; }
        public TimeSpan? BreakEndTime { get; set; }


        public int UserID { get; set; }

        

        //public ICollection<Appointment> Appointments { get; set; }
        //public ICollection<Patient_Encounter> Encounters { get; set; }
    }
}
