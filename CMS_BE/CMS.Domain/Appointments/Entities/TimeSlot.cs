using CMS.Domain.Clinic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.Appointments.Entities
{
    public class TimeSlot
    {
        public Guid SlotID { get; set; }
        public Guid DoctorID { get; set; }
        public DateTime SlotDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }


        //public Doctor Doctor { get; set; }
    }
}
