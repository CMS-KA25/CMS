using CMS.Domain.Appointments.Entities;
using CMS.Domain.EMR.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.EMR.Entities
{
    public class Patient
    {
        public Guid PatientID { get; set; }
        public DateOnly Date_Of_Birth { get; set; }
        public SexType Sex { get; set; }
        public string Address { get; set; }
        public string Blood_Group { get; set; }
        public string Allergies { get; set; }


        //public ICollection<Appointment> Appointments { get; set; }
        //public ICollection<Patient_Encounter> Encounters { get; set; }
    }
}
