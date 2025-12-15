using CMS.Domain.Appointments.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.Clinic.Entities
{
    public class Patient_Encounter
    {
        public Guid EncounterID { get; set; }
        public Guid PatientID { get; set; }
        public Guid DoctorID { get; set; }
        //public DateOnly Encounter_date { get; set; }
        //public string Encounter_type { get; set; }
        //public string Reason_description { get; set; }
        public string Description { get; set; }
        public Guid? Parent_Encounter_ID { get; set; }
        public Guid AppointmentID { get; set; }


        //public Patient Patient { get; set; }
        //public Doctor Doctor { get; set; }
        //public Appointment Appointment { get; set; }
        //public ICollection<Observations> Observations { get; set; }
        //public ICollection<MedicalReports> MedicalReports { get; set; }
        //public ICollection<Prescriptions> Prescriptions { get; set; }
        //public ICollection<Patient_Bills> Bills { get; set; }
    }
}
