using CMS.Domain.Clinic.Entities;
using CMS.Domain.EMR.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.EMR.Entities
{
    public class Prescription
    {
        public Guid PrescriptionID { get; set; }
        public Guid EncounterID { get; set; }
        public Guid DoctorID { get; set; }
        [Required]
        public string MedicationName { get; set; }
        [Required]
        public int Dosage { get; set; }
        public string Unit { get; set; }
        [Required]
        public MedicationFrequency Frequency { get; set; }
        [Required]
        public string Duration { get; set; }
        [Required]
        public string Notes { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; }


        //public Patient_Encounter Encounter { get; set; }
        //public Doctor Doctor { get; set; }
    }
}
