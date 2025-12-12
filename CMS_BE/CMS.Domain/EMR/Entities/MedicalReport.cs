using CMS.Domain.Auth.Entities;
using CMS.Domain.EMR.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.EMR.Entities
{
    public class MedicalReport
    {
        public Guid ReportId { get; set; }
        public Guid PatientId { get; set; }
        public Guid EncounterId { get; set; }
        public string FileUrl { get; set; }
        [Required]
        public ReportType ReportType { get; set; }
        [Required]
        public string Findings { get; set; }
        public Guid UploadedBy { get; set; }
        [Required]
        public DateTime DateUploaded { get; set; }


        //public Patient Patient { get; set; }
        //public Patient_Encounter Encounter { get; set; }
        //public User Uploader { get; set; }
    }
}
