using CMS.Domain.Appointments.Enums;
using CMS.Domain.Auth.Entities;
using CMS.Domain.Clinic.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.Appointments.Entities
{
    public class Appointment
    {
        public Guid AppointmentID { get; set; }
        public Guid PatientID { get; set; }
        public Guid DoctorID { get; set; }
        //public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        [Required]
        public AppointmentStatus Status { get; set; }
        public Guid SlotID { get; set; }
        [Required]
        public AppointmentType AppointmentType { get; set; }
        public string GoogleCalendarEventID { get; set; }
        [Required]
        public string Reason_description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid CreatedBy { get; set; }


        //public Patient Patient { get; set; }
        //public Doctor Doctor { get; set; }
        //public Time_Slots Slot { get; set; }
        //public User CreatedUser { get; set; }
    }
}
