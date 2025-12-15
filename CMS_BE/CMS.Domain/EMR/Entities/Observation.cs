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
        public Guid ObservationId { get; set; }
        public Guid Encounter_Id { get; set; }
        public string Observations_Name { get; set; }
        public string Reference_range { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
        public DateTime DateRecorded { get; set; }
        public Guid Staff_id { get; set; }


        //public Patient_Encounter Encounter { get; set; }
        //public User Staff { get; set; }
    }
}
