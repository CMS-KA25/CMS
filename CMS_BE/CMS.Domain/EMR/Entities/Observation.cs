using CMS.Domain.Auth.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.EMR.Entities
{
    public class Observation
    {
        public Guid ObservationID { get; set; }
        public Guid EncounterID { get; set; }
        public string ObservationName { get; set; }
        public string ReferenceRange { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
        public DateTime DateRecorded { get; set; }
        public Guid StaffID { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; }


        //public Patient_Encounter Encounter { get; set; }
        //public User Staff { get; set; }
    }
}
