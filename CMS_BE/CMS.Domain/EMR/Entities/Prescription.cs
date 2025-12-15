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
    public class Prescriptions
    {
        public Guid PrescriptionId { get; set; }
        public Guid EncounterId { get; set; }
        public Guid DoctorId { get; set; }
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


        //public Patient_Encounter Encounter { get; set; }
        //public Doctor Doctor { get; set; }
    }
}
