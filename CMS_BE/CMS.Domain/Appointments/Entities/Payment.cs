using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Domain
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid payment_id { get; set; }
        
        public string razorpay_order_id { get; set; } = string.Empty;
        public string razorpay_payment_id { get; set; } = string.Empty;
        public string razorpay_signature { get; set; } = string.Empty;
        
        public decimal amount { get; set; }
        public decimal original_amount { get; set; }
        public decimal discount_amount { get; set; }
        public string currency { get; set; } = "INR";
        public string status { get; set; } = "pending"; // pending, success, failed
        public bool is_followup { get; set; } = false;
        
        public Guid? patient_id { get; set; }
        public string description { get; set; } = string.Empty;
        public string? bill_pdf_path { get; set; }
        
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        public DateTime? updated_at { get; set; }
        
        // Navigation property
        public Patient? Patient { get; set; }
    }
}