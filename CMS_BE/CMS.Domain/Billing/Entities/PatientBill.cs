using CMS.Domain.Auth.Entities;
using CMS.Domain.Billing.Enums;
using CMS.Domain.Clinic.Entities;
using CMS.Domain.EMR.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.Billing.Entities
{
    public class PatientBill
    {
        public Guid BillID { get; set; }
        public Guid EncounterID { get; set; }
        public Guid TemplateID { get; set; }
        //public string BillType { get; set; }
        
        public string Description { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public decimal DiscountAmount { get; set; } = 0;
        [Required]
        public decimal FinalAmount { get; set; }
        public DateTime BillDate { get; set; }
        public BillStatusType Status { get; set; } 
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }


        //public Patient Patient { get; set; }
        //public Patient_Encounter Encounter { get; set; }
        //public Bill_Templates Template { get; set; }
        //public Doctor Doctor { get; set; }
        //public User Creator { get; set; }
    }
}
