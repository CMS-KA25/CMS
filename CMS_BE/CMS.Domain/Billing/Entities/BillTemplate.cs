using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.Billing.Entities
{
    public class BillTemplate
    {
        public Guid TemplateID { get; set; }
        [Required]
        public string TemplateName { get; set; }
        [Required]
        public string ServiceDescription { get; set; }
        [Required]
        public decimal BaseAmount { get; set; }
        [Required]
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }


        //public ICollection<Patient_Bills> Bills { get; set; }
    }
}
