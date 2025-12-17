using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Domain
{
    public class Patient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid patient_id { get; set; }
        
        public DateOnly date_of_birth { get; set; }
        public char sex { get; set; }
        public string country { get; set; } = string.Empty;
        public string pincode { get; set; } = string.Empty;
        public string city { get; set; } = string.Empty;
        public string state { get; set; } = string.Empty;
        
        public string? address { get; set; }
        
        public string? marital_status { get; set; }
        
        [Required]
        public string blood_group { get; set; } = string.Empty;
        
        public string? allergies { get; set; }
        
        public string? chief_medical_complaints { get; set; }
        
        public bool consulted_before { get; set; } = false;
        
        public string? doctor_name { get; set; }
        
        public DateOnly? last_review_date { get; set; }
        
        public bool seeking_followup { get; set; } = false;
        
        public string? profile_image_path { get; set; }
        
        public string? medical_reports_path { get; set; }
    }
}

