using CMS.Domain.Appointments.Entities;
using CMS.Domain.EMR.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.EMR.Entities
{
    public class Patient
    {
        public Guid PatientID { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public SexType Sex { get; set; }
        public string Address { get; set; }
        public string BloodGroup { get; set; }
        public string Allergies { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }


        //public ICollection<Appointment> Appointments { get; set; }
        //public ICollection<Patient_Encounter> Encounters { get; set; }
    }
}
