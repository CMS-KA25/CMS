using CMS.Domain.Auth.Entities;
using CMS.Domain.Clinic.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.Clinic.Entities
{
    public class Leave
    {
        public Guid LeaveID { get; set; }
        public Guid UserID { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public string Reason { get; set; }
        [Required]
        public LeaveStatus Status { get; set; } 
        public DateTime LeaveApplied { get; set; }
        public bool IsFullDay { get; set; } = true;
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }


        //public User User { get; set; }
    }
}
