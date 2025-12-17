namespace CMS.Models
{
    public class PatientDto
    {
        public string first_name { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string country_code { get; set; } = "+91";
        public string phone_number { get; set; } = string.Empty;
        public string date_of_birth { get; set; } = string.Empty;
        public string sex { get; set; } = "M";
        public string country { get; set; } = string.Empty;
        public string pincode { get; set; } = string.Empty;
        public string city { get; set; } = string.Empty;
        public string state { get; set; } = string.Empty;
        public string? address { get; set; }
        public string? marital_status { get; set; }
        public string? blood_group { get; set; }
        public string? allergies { get; set; }
        public string? chief_medical_complaints { get; set; }
        public bool consulted_before { get; set; } = false;
        public string? doctor_name { get; set; }
        public string? last_review_date { get; set; }
        public bool seeking_followup { get; set; } = false;
        public string? profile_image_path { get; set; }
        public string? medical_reports_path { get; set; }
    }
}